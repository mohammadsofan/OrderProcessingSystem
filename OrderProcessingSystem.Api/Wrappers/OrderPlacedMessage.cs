using OrderProcessingSystem.Api.Dtos.Responses;
using RabbitMQ.Client.Exceptions;

namespace OrderProcessingSystem.Api.Wrappers
{
    public class OrderPlacedMessage
    {
        public string Email { get; set; } = string.Empty;
        public OrderResponseDto? Order { get; set; }
    }
}
