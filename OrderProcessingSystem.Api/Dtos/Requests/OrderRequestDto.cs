using OrderProcessingSystem.Api.Enums;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Dtos.Requests
{
    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

    }
}
