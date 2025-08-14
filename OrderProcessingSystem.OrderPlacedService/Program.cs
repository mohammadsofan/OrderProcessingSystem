using OrderProcessingSystem.OrderPlacedService.Services;

namespace OrderProcessingSystem.OrderPlacedService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            var host = builder.Build();
            host.Run();
        }
    }
}