using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CartItem>()
                .HasIndex(CartItem => new { CartItem.UserId, CartItem.ProductId }).IsUnique();
            modelBuilder.Entity<OrderItem>()
                .HasIndex(OrderItem => new { OrderItem.OrderId, OrderItem.ProductId }).IsUnique();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

    }
}
