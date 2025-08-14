using OrderProcessingSystem.Api.Interfaces.IService;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderProcessingSystem.Api.Services
{
    public class MessageBrokerService:IMessageBrokerService
    {
        public async Task SendMessage<T>(T message)
        {
            var connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672"),
                ClientProvidedName = "OrderProcessingSystem.Api"
            };

            var connection = await connectionFactory.CreateConnectionAsync();

            var exchangeName = "order_exchange";
            var queueName = "order_queue";
            var routingKey = "order_routing_key";

            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queueName, exchangeName, routingKey);

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            await channel.BasicPublishAsync(exchangeName, routingKey,body);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
