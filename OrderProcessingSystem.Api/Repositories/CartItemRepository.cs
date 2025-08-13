using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Repositories
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
