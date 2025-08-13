using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Dtos.Responses;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Services
{
    public class OrderItemService:Service<OrderItemRequestDto, OrderItemResponseDto, OrderItem>, IOrderItemService
    {
        private readonly IRepository<OrderItem> _repository;
        public OrderItemService(IRepository<OrderItem> repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
