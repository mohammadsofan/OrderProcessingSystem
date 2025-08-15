using OrderProcessingSystem.OrderPlacedService.Services;
using Serilog;
using System.Linq.Expressions;

namespace OrderProcessingSystem.OrderPlacedService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateBootstrapLogger();
            var logger = Log.ForContext<Program>();
            logger.Information("Starting up OrderPlacedService");
            try
            {
                var builder = Host.CreateApplicationBuilder(args);
                builder.Services.AddSerilog((services, loggerConfig) =>
                {
                    loggerConfig.ReadFrom.Configuration(builder.Configuration);
                });
                builder.Services.AddHostedService<Worker>();
                builder.Services.AddScoped<IEmailSender, EmailSender>();
                var host = builder.Build();
                host.Run();
                logger.Information("OrderPlacedService Stopped cleanly");

            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "An unhandled exception occurred during bootstrapping");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}