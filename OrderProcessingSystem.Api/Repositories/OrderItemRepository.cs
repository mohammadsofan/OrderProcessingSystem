using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Repositories
{
    public class OrderItemRepository:Repository<OrderItem>, IOrderItemRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
