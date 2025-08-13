using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Models;
using OrderProcessingSystem.Api.Wrappers;

namespace OrderProcessingSystem.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        public Task<ServiceResult<object>> LoginUserAsync(RegisterUserRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> RegisterUserAsync(RegisterUserRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
