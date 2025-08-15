using OrderProcessingSystem.Api.Interfaces.IService;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderProcessingSystem.Api.Services
{
    public class MessageBrokerService : IMessageBrokerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessageBrokerService> _logger;

        public MessageBrokerService(IConfiguration configuration, ILogger<MessageBrokerService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SendMessage<T>(T message)
        {
            try
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = _configuration["RabbitMQ:Host"]!,
                    UserName = _configuration["RabbitMQ:User"]!,
                    Password = _configuration["RabbitMQ:Pass"]!,
                    ClientProvidedName = "OrderProcessingSystem.Api"
                };

                using var connection = await connectionFactory.CreateConnectionAsync();

                var exchangeName = "order_exchange";
                var queueName = "order_queue";
                var routingKey = "order_routing_key";

                using var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
                await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
                await channel.QueueBindAsync(queueName, exchangeName, routingKey);

                var jsonString = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonString);

                await channel.BasicPublishAsync(exchangeName, routingKey, body);

                _logger.LogInformation("Message published to RabbitMQ. Exchange: {Exchange}, RoutingKey: {RoutingKey}", exchangeName, routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to RabbitMQ.");
                throw;
            }
        }
    }
}
