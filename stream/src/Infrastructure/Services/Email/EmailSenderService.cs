using System.Net;
using System.Net.Mail;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        //Di for config
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            //get config first from the settings
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = _configuration["Smtp:Port"];
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Pass"];
            var fromEmail = _configuration["Smtp:FromEmail"];

            //use the client for the connections
            using var client = new SmtpClient(smtpHost, int.Parse(smtpPort))
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            //for sending now 

            var mail = new MailMessage(fromEmail, toEmail, subject, htmlMessage)
            {
                IsBodyHtml = true,
            };

            // DO THE THIN now
            await client.SendMailAsync(mail);

        }
    }
}