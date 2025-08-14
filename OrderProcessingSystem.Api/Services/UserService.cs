using Microsoft.AspNetCore.Identity;
using OrderProcessingSystem.Api.Constants;
using OrderProcessingSystem.Api.Dtos.Requests;
using OrderProcessingSystem.Api.Enums;
using OrderProcessingSystem.Api.Exceptions;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Models;
using OrderProcessingSystem.Api.Wrappers;

namespace OrderProcessingSystem.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<object> _passwordHasher;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository repository,
            IPasswordHasher<object> passwordHasher,
            ITokenService tokenService)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }
        public async Task<ServiceResult<object>> LoginUserAsync(LoginUserRequestDto request)
        {
            try
            {
                var user = await _repository.GetOneByFilterAsync(u => u.Email.ToLower() == request.Email.ToLower());
                if (user == null)
                {
                    return ServiceResult<object>.Failure("Invalid email or password.",ServiceResultStatus.BadRequest);
                }
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(request, user.HashedPassword, request.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    return ServiceResult<object>.Failure("Invalid email or password.", ServiceResultStatus.BadRequest);
                }

                var token = _tokenService.GenerateToken(
                    user.Id.ToString(),
                    user.UserName,
                    user.Email,
                    user.Role,
                    DateTime.UtcNow.AddHours(1)
                );
                return ServiceResult<object>.Success(new { Token = token }, "User Logged in successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Failure($"Failure with login process: {ex.Message}");
            }
        }
        public async Task<ServiceResult> RegisterUserAsync(RegisterUserRequestDto request)
        {
            try
            {
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Email = request.Email,
                    HashedPassword = _passwordHasher.HashPassword(request, request.Password),
                    Role = ApplicationRoles.User
                };
                await _repository.AddAsync(user);
                return ServiceResult.Success("User registered successfully.");
            }
            catch (ConflictDbException ex)
            {
                return ServiceResult.Failure($"Conflict error: {ex.Message}", ServiceResultStatus.Conflict);
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Failure with register proccess :{ex.Message}");
            }
        }
    }
}
