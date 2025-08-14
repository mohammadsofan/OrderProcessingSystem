using OrderProcessingSystem.Api.Enums;
using OrderProcessingSystem.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace OrderProcessingSystem.Api.Dtos.Requests
{
    public class OrderRequestDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be a positive value.")]
        public decimal TotalAmount { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Shipping address is required.")]
        public string ShippingAddress { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

    }
}
