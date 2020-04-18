using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CardsAPI.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailSender(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;

        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var from = _emailConfiguration.From;
                using (var client = new SmtpClient())
                {
                    MailMessage mail = new MailMessage(from, email);
                    mail.From = new MailAddress(_emailConfiguration.From);
                    mail.Subject = subject;
                    mail.Body = message;
                    client.Port = _emailConfiguration.Port;
                    client.Host = _emailConfiguration.SmtpServer;
                    client.Credentials = new System.Net.NetworkCredential(_emailConfiguration.Username, _emailConfiguration.Password);
                    client.EnableSsl = true;
                    client.Send(mail);
                    mail.Dispose();
                    
                }
            }
            catch(Exception ex)
            {
                //Handle failures
            }
        }

    }
}
