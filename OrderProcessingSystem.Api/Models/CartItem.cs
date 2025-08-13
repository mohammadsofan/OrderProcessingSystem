using OrderProcessingSystem.Api.Interfaces;

namespace OrderProcessingSystem.Api.Models
{
    public class CartItem: IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
