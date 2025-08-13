using Microsoft.IdentityModel.Tokens;
using OrderProcessingSystem.Api.Interfaces.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrderProcessingSystem.Api.Services
{
    public class TokenService:ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId, string userName, string email, string role, DateTime expirationDate)
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
                throw new InvalidOperationException("JWT Key is not configured.");
            }
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfigKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expirationDate,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
