using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CardsAPI.Controllers;
using CardsAPI.Models;
using CardsAPITests.MockClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        private Mock<FakeUserManager> _userManagerMock;
        private Mock<FakeSignInManager> _signInManagerMock;
        private Mock<IEmailSender> _emailSender;
        private LoginController _loginController;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<FakeUserManager>();
            _signInManagerMock = new Mock<FakeSignInManager>();
            _emailSender = new Mock<IEmailSender>();
            _loginController = new LoginController(_userManagerMock.Object, _signInManagerMock.Object, _emailSender.Object);
        }

        [Test]
        public void When_signup_method_is_called_with_data_then_return_created_user()
        {
            // Arrange
            UserUpsertion userUpsertion = new UserUpsertion();
            userUpsertion.Name = "User1";
            userUpsertion.MailId = "user@example.com";
            userUpsertion.Password = "123";

            
            _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<IdentityUser>(), userUpsertion.Password)).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var response = _loginController.Signup(userUpsertion);

            //// Assert
            Assert.IsNotNull(response.Result.Result);
        }

        [Test]
        [TestCase("user1","","")]
        [TestCase("user1", "user@example.com", "")]
        [TestCase("", "", "")]
        public void When_signup_method_is_called_with_empty_parameters_then_return_bad_request(string name,string mailId,string password)
        {
            UserUpsertion userUpsertion = new UserUpsertion();
            userUpsertion.Name = name;
            userUpsertion.MailId = mailId;
            userUpsertion.Password = password;

            // Act
            var response = _loginController.Signup(userUpsertion);
            var objectResult = response.Result.Result as ObjectResult;

            //// Assert
            Assert.AreEqual(objectResult.StatusCode.Value, (int)HttpStatusCode.BadRequest);
        }
    }
}
