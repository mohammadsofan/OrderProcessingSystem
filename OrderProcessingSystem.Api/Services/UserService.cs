using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository repository,
            IPasswordHasher<object> passwordHasher,
            ITokenService tokenService, 
            ILogger<UserService> logger)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }
        public async Task<ServiceResult<object>> LoginUserAsync(LoginUserRequestDto request)
        {
            try
            {
                _logger.LogInformation("Attempting login for email: {Email}", request.Email);
                var user = await _repository.GetOneByFilterAsync(u => u.Email.ToLower() == request.Email.ToLower());
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User with email {Email} not found.", request.Email);
                    return ServiceResult<object>.Failure("Invalid email or password.",ServiceResultStatus.BadRequest);
                }
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(request, user.HashedPassword, request.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Login failed: Invalid password for email {Email}.", request.Email);
                    return ServiceResult<object>.Failure("Invalid email or password.", ServiceResultStatus.BadRequest);
                }

                var token = _tokenService.GenerateToken(
                    user.Id.ToString(),
                    user.UserName,
                    user.Email,
                    user.Role,
                    DateTime.UtcNow.AddHours(1)
                );
                _logger.LogInformation("User {Email} logged in successfully.", request.Email);
                return ServiceResult<object>.Success(new { Token = token }, "User Logged in successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure with login process for email: {Email}", request.Email);
                return ServiceResult<object>.Failure($"Failure with login process: {ex.Message}");
            }
        }
        public async Task<ServiceResult> RegisterUserAsync(RegisterUserRequestDto request)
        {
            try
            {
                _logger.LogInformation("Attempting registration for email: {Email}", request.Email);
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
                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return ServiceResult.Success("User registered successfully.");
            }
            catch (ConflictDbException ex)
            {
                _logger.LogWarning(ex, "Conflict error during registration for email: {Email}", request.Email);
                return ServiceResult.Failure($"Conflict error: {ex.Message}", ServiceResultStatus.Conflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure with register process for email: {Email}", request.Email);
                return ServiceResult.Failure($"Failure with register proccess :{ex.Message}");
            }
        }
    }
}
