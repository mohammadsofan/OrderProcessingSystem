namespace OrderProcessingSystem.Api.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // e.g., Pending, Completed, Cancelled
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
