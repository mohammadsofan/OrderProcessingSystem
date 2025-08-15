using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Api.Interfaces.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrderProcessingSystem.Api.Services
{
    public class TokenService:ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(string userId, string userName, string email, string role, DateTime expirationDate)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                };
                var jwtConfigKey = _configuration["Jwt:Key"];
                if(string.IsNullOrEmpty(jwtConfigKey))
                {
                    _logger.LogError("JWT Key is not configured.");
                    throw new InvalidOperationException("JWT Key is not configured.");
                }
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfigKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: creds
                );
                _logger.LogInformation("JWT token generated for user {UserId} with role {Role}.", userId, role);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {UserId}.", userId);
                throw;
            }
        }
    }
}
