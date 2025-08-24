using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Repositories
{
    public class ProductRepostitory : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepostitory(ApplicationDbContext context,ILogger<ProductRepostitory> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
