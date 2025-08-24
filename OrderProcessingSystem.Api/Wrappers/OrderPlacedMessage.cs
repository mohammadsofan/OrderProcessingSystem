using OrderProcessingSystem.Api.Dtos.Responses;
using RabbitMQ.Client.Exceptions;

namespace OrderProcessingSystem.Api.Wrappers
{
    public class OrderPlacedMessage
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime PlacedAt { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public IList<OrderPlacedItem> Items { get; set; } = new List<OrderPlacedItem>();

    }
    public class OrderPlacedItem
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
