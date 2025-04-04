using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CarRental.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string token)
        {
            var fromAddress = new MailAddress(_configuration["Brevo:SmtpEmail"], "YourApp");
            var toAddress = new MailAddress(email);
            string subject = "Verify Your Account";
            string body = $"Click the link to verify your account: https://yourapp.com/verify?token={token}";

            using var smtp = new SmtpClient
            {
                Host = "smtp-relay.brevo.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(_configuration["Brevo:SmtpEmail"], _configuration["Brevo:SmtpPassword"])
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}