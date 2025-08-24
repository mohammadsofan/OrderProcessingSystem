using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Dtos.Responses
{
    public class CartItemResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public ProductResponseDto? Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
