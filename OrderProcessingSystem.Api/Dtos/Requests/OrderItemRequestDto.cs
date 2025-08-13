using OrderProcessingSystem.Api.Dtos.Responses;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Dtos.Requests
{
    public class OrderItemRequestDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
