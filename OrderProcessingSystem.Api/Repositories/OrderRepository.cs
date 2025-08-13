using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Repositories
{
    public class OrderRepository: Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
    {
    }
}
