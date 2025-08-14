using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.OrderPlacedService.Services
{
    internal class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration )
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io")
            {
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(_configuration["EmailSender:UserName"], _configuration["EmailSender:Password"]),
                EnableSsl = true,
                
            };
            var message = new MailMessage("sofan@gmail.com", to, subject, body) { 
            IsBodyHtml = true
            };
            await smtpClient.SendMailAsync(message);
        }
    }
}
