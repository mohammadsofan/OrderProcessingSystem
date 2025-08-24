using System;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace OrderProcessingSystem.OrderPlacedService.Services
{
    internal class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(_configuration["EmailSender:UserName"], _configuration["EmailSender:Password"]),
                    EnableSsl = true,
                };
                var message = new MailMessage("sofan@gmail.com", to, subject, body)
                {
                    IsBodyHtml = true
                };
                await smtpClient.SendMailAsync(message);
                _logger.LogInformation("Email sent to {To} with subject '{Subject}'", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", to, subject);
                throw;
            }
        }
    }
}
