using System;
using CardsAPI.Models;
using NUnit.Framework;

namespace CardsAPITests.ModelsTest
{
    public class UserTest
    {
        [Test]
        public void When_email_configuration_object_is_created_then_validate_if_all_properties_are_set()
        {
            string id = "1";
            string mailId = "user@example.com";
            string name = "User";
            string password = "password";

            User user = new User
            {
                Id = id,
                MailId = mailId,
                Name = name,
                Password = password
            };

            Assert.AreSame(user.Id, id);
            Assert.AreSame(user.MailId, mailId);
            Assert.AreEqual(user.Name, name);
            Assert.AreSame(user.Password, password);
        }
    }
}
