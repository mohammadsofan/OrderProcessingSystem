using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Dtos.Responses;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Models;
using OrderProcessingSystem.Api.Wrappers;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Api.Services
{
    public class OrderService:Service<OrderRequestDto, OrderResponseDto, Order>, IOrderService
    {
        private readonly IRepository<Order> _repository;
        public OrderService(IRepository<Order> repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
