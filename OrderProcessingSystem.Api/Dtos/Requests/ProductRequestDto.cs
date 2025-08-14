using System.ComponentModel.DataAnnotations;

namespace OrderProcessingSystem.Api.Dtos.Requests
{
    public class ProductRequestDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [MinLength(3, ErrorMessage = "Product name must be at least 3 characters long.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Product description is required.")]
        [MinLength(3, ErrorMessage = "Product description must be at least 3 characters long.")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative integer.")]
        public int StockQuantity { get; set; }
    }
}
