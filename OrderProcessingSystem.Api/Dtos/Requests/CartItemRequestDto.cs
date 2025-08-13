using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Dtos.Requests
{
    public class CartItemRequestDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
