using Microsoft.EntityFrameworkCore;

namespace OrderProcessingSystem.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }

    }
}
