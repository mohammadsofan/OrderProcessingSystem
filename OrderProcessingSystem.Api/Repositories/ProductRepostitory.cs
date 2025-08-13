using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Repositories
{
    public class ProductRepostitory : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepostitory(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
