using OrderProcessingSystem.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace OrderProcessingSystem.Api.Dtos.Requests
{
    public class CartItemRequestDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal TotalPrice { get; set; }
    }
}
