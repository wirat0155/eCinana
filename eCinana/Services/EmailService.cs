using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System;

namespace eCinana.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendPasswordResetEmail(string email, string resetLink)
        {
            try
            {
                var smtpHost = _configuration["Email:Host"];
                var smtpPort = int.Parse(_configuration["Email:Port"]);
                var emailAddress = _configuration["Email:Username"];
                var emailPassword = _configuration["Email:Password"];

                var subject = "Password Reset Request";
                var body = $"Click the following link to reset your password: {resetLink}";

                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(emailAddress, emailPassword)
                };

                var mailMessage = new MailMessage(emailAddress, email, subject, body);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
