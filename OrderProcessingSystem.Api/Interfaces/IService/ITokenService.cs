namespace OrderProcessingSystem.Api.Interfaces.IService
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string userName,string email, string role, DateTime expirationDate);
    }
}
