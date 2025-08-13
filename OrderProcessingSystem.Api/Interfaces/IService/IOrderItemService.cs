using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Dtos.Responses;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Interfaces.IService
{
    public interface IOrderItemService: IService<OrderItemRequestDto, OrderItemResponseDto, OrderItem>
    {
    }
}
