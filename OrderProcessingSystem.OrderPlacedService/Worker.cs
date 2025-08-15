using OrderProcessingSystem.OrderPlacedService.Messages;
using OrderProcessingSystem.OrderPlacedService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace OrderProcessingSystem.OrderPlacedService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration, ILogger<Worker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var connectionFactory = new ConnectionFactory()
                    {
                        HostName = _configuration["RabbitMQ:Host"]!,
                        UserName = _configuration["RabbitMQ:User"]!,
                        Password = _configuration["RabbitMQ:Pass"]!,
                    };
                    var connection = await connectionFactory.CreateConnectionAsync();
                    var channel = await connection.CreateChannelAsync();
                    var exchangeName = "order_exchange";
                    var queueName = "order_queue";
                    var routingKey = "order_routing_key";
                    await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, true, false);
                    await channel.QueueDeclareAsync(queueName, true, false, false);
                    await channel.QueueBindAsync(queueName, exchangeName, routingKey);
                    await channel.BasicQosAsync(0, 1, false);
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (sender, arg) =>
                    {
                        try
                        {
                            var body = arg.Body.ToArray();
                            var jsonMessage = System.Text.Encoding.UTF8.GetString(body);
                            var orderPlacedMessage = JsonSerializer.Deserialize<OrderPlacedMessage>(jsonMessage);
                            using var scope = _serviceScopeFactory.CreateScope();
                            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                            await channel.BasicAckAsync(arg.DeliveryTag, false);
                            await emailSender.SendEmailAsync(
                                orderPlacedMessage!.Email,
                                "Order Confirmation - Thank you for your order!",
                                $@"
                                <html>
                                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                                    <h2 style='color: #2E86C1;'>Hello {orderPlacedMessage?.UserName},</h2>
                                    <p>Thank you for your order placed on <strong>{orderPlacedMessage?.PlacedAt:yyyy-MM-dd HH:mm}</strong>.</p>
                                    <p>We are preparing your items for shipment to:</p>
                                    <blockquote>
                                        {orderPlacedMessage?.ShippingAddress}<br/>
                                        Phone: {orderPlacedMessage?.PhoneNumber}
                                    </blockquote>
                                    <h3>Order Details:</h3>
                                    <table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse; width: 100%;'>
                                        <thead>
                                            <tr style='background-color: #f2f2f2;'>
                                                <th>Product</th>
                                                <th>Quantity</th>
                                                <th>Price</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {string.Join("", orderPlacedMessage!.Items.Select(item =>
                                                $"<tr><td>{item.ProductName}</td><td>{item.Quantity}</td><td>{item.Price:$}</td></tr>"
                                            ))}
                                        </tbody>
                                    </table>
                                    <p style='font-size: 1.1em;'><strong>Total Amount:</strong> {orderPlacedMessage?.TotalAmount:$}</p>
                                    <p>We will notify you once your order has been shipped.</p>
                                    <p style='color: gray;'>If you have any questions, feel free to reply to this email.</p>
                                    <p>Best regards,<br/>Our Store Team</p>
                                </body>
                                </html>
                                "
                            );
                            _logger.LogInformation("Processed order placed message and sent email to {Email}", orderPlacedMessage.Email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing order placed message from RabbitMQ.");
                            await channel.BasicNackAsync(arg.DeliveryTag, false, true);
                        }
                    };
                    string consumerTag = await channel.BasicConsumeAsync(queueName, false, consumer);
                    _logger.LogInformation("Started consuming messages from RabbitMQ queue: {Queue}", queueName);
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Worker main loop.");
                }
            }
        }
    }
}
