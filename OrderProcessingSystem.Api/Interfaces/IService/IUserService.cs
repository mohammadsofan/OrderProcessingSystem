using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Wrappers;

namespace OrderProcessingSystem.Api.Interfaces.IService
{
    public interface IUserService
    {
        Task<ServiceResult> RegisterUserAsync(RegisterUserRequestDto request);
        Task<ServiceResult<object>> LoginUserAsync(RegisterUserRequestDto request);
    }
}
