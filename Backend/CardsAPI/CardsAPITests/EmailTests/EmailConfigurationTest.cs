using System;
using CardsAPI.Email;
using NUnit.Framework;

namespace CardsAPITests.EmailTests
{
    public class EmailConfigurationTest
    {
        [Test]
        public void When_email_configuration_object_is_created_then_validate_if_all_properties_are_set()
        {
            string from = "user@example.com";
            string smtpServer = "smtpServer";
            int port = 50;
            string username = "username";
            string password = "password" ;

            EmailConfiguration emailConfiguration = new EmailConfiguration();
            emailConfiguration.From = from;
            emailConfiguration.SmtpServer = smtpServer;
            emailConfiguration.Port = port;
            emailConfiguration.Username = username;
            emailConfiguration.Password = password;

            Assert.AreSame(emailConfiguration.From, from);
            Assert.AreSame(emailConfiguration.SmtpServer, smtpServer);
            Assert.AreEqual(emailConfiguration.Port, port);
            Assert.AreSame(emailConfiguration.Username, username);
            Assert.AreSame(emailConfiguration.Password, password);
        }
    }
}
