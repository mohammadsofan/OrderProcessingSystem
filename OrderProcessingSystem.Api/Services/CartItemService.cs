using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Dtos.Responses;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Services
{
    public class CartItemService:Service<CartItemRequestDto, CartItemResponseDto, CartItem>, ICartItemService
    {
        private readonly ICartItemRepository _repository;
        public CartItemService(ICartItemRepository repository,ILogger<CartItemService> logger) : base(repository, logger)
        {
            _repository = repository;
        }
    }
}
