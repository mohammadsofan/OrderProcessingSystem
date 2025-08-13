using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Api.Constants;
using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Interfaces;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Utils
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<object> passwordHasher;

        public DBInitializer(ApplicationDbContext context,IPasswordHasher<object> passwordHasher)
        {
           _context = context;
            this.passwordHasher = passwordHasher;
        }
        public async Task InitializeAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
                if (!_context.Users.Any())
                {
                    var user = new User()
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        UserName = "Admin",
                        Email = "admin@admin.com",
                        HashedPassword = passwordHasher.HashPassword(null!, "Admin@123"),
                        Role = ApplicationRoles.Admin,
                    };
                    await _context.Users.AddAsync(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while initializing the database.", ex);
            }
        }
    }
}
