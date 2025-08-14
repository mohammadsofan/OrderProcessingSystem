using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.OrderPlacedService.Services
{
    internal interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
