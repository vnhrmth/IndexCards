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
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Tests
{
    public class LoginTests
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
        public void When_signup_method_is_called_with_data_then_return_created_status_code()
        {
            // Arrange
            UserUpsertion userUpsertion = new UserUpsertion();
            userUpsertion.Name = "User1";
            userUpsertion.MailId = "user@example.com";
            userUpsertion.Password = "123";


            _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<IdentityUser>(), userUpsertion.Password)).
            Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var response = _loginController.Signup(userUpsertion);

            ObjectResult objectResult = response.Result as ObjectResult;

            //// Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.Created);
        }

        [Test]
        [TestCase("user1", "", "")]
        [TestCase("user1", "user@example.com", "")]
        [TestCase("", "", "")]
        public void When_signup_method_is_called_with_empty_parameters_then_return_bad_request(string name, string mailId, string password)
        {
            UserUpsertion userUpsertion = new UserUpsertion();
            userUpsertion.Name = name;
            userUpsertion.MailId = mailId;
            userUpsertion.Password = password;

            // Act
            var response = _loginController.Signup(userUpsertion);
            var objectResult = response.Result as ObjectResult;

            //// Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Test]
        public void When_signup_method_is_called_with_failure_result_then_return_bad_request()
        {
            UserUpsertion userUpsertion = new UserUpsertion();
            userUpsertion.Name = "V";
            userUpsertion.MailId = "v@example.com";
            userUpsertion.Password = "123";

            _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<IdentityUser>(),
            "123")).Returns(Task.FromResult(IdentityResult.Failed(new IdentityError())));

            // Act
            var response = _loginController.Signup(userUpsertion);
            var objectResult = response.Result as ObjectResult;

            //// Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Test]
        public void When_login_method_is_called_with_credentials_then_return_Ok_status_code()
        {
            // Arrange
            LoginUserUpsertion loginUserUpsertion = new LoginUserUpsertion();
            loginUserUpsertion.MailId = "user@example.com";
            loginUserUpsertion.Password = "123";

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(loginUserUpsertion.MailId)).
            Returns(Task.FromResult(new IdentityUser()));

            _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<bool>())).
            Returns(Task.FromResult(SignInResult.Success));

            _signInManagerMock.Setup(x =>
            x.PasswordSignInAsync(It.IsAny<string>(),
                                  It.IsAny<string>(),
                                  It.IsAny<bool>(),
                                  It.IsAny<bool>())).
            Returns(Task.FromResult(SignInResult.Success));

            // Act
            var response = _loginController.Signin(loginUserUpsertion);
            ObjectResult objectResult = response.Result as ObjectResult;

            //// Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.OK);
        }

        [Test]
        [TestCase("", "")]
        [TestCase("user@example.com","")]
        [TestCase("", "123")]
        [TestCase("user@example","123")]
        public void When_login_method_is_called_with_wrong_format_then_return_bad_request_status_code(string emailId,string password)
        {
            // Arrange
            LoginUserUpsertion loginUserUpsertion = new LoginUserUpsertion();
            loginUserUpsertion.MailId = emailId;
            loginUserUpsertion.Password = password;

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(loginUserUpsertion.MailId)).
            Returns(Task.FromResult(new IdentityUser()));

            // Act
            var response = _loginController.Signin(loginUserUpsertion);
            ObjectResult objectResult = response.Result as ObjectResult;

            //// Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.BadRequest);
        }
    }
}

