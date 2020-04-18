using System;
namespace CardsAPI.Email
{
    public interface IEmailConfiguration
    {
        string From { get; set; }
        string SmtpServer { get; }
        int Port { get; }
        string Username { get; }
        string Password { get; }
    }

}