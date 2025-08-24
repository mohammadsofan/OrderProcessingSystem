using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Api.Constants;
using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Interfaces;
using OrderProcessingSystem.Api.Models;

namespace OrderProcessingSystem.Api.Utils
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<object> _passwordHasher;
        private readonly ILogger<DBInitializer> _logger;

        public DBInitializer(ApplicationDbContext context, IPasswordHasher<object> passwordHasher, ILogger<DBInitializer> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Starting database migration and initialization.");
                await _context.Database.MigrateAsync();
                if (!_context.Users.Any())
                {
                    var user = new User()
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        UserName = "Admin",
                        Email = "admin@admin.com",
                        HashedPassword = _passwordHasher.HashPassword(null!, "Admin@123"),
                        Role = ApplicationRoles.Admin,
                    };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Default admin user created: {Email}", user.Email);
                }
                else
                {
                    _logger.LogInformation("Admin user already exists. Skipping creation.");
                }
                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw new Exception("An error occurred while initializing the database.", ex);
            }
        }
    }
}
