using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Dtos.Responses;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Services
{
    public class ProductService : Service<ProductRequestDto, ProductResponseDto, Product>, IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository,ILogger<ProductService> logger) : base(repository, logger)
        {
            _repository = repository;
        }

    }
}
