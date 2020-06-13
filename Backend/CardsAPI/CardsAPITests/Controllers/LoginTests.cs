using System;
using System.Net;
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

namespace CardsAPITests.Controllers
{
    public class LoginTests
    {
        private Mock<FakeUserManager> _userManagerMock;
        private Mock<FakeSignInManager> _signInManagerMock;
        private Mock<IEmailSender> _emailSender;
        private LoginController _loginController;
        private Mock<FakeAppSettings> _appSettings;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<FakeUserManager>();
            _signInManagerMock = new Mock<FakeSignInManager>();
            _emailSender = new Mock<IEmailSender>();
            _appSettings = new Mock<FakeAppSettings>();
            _loginController = new LoginController(_userManagerMock.Object, _signInManagerMock.Object, _emailSender.Object, _appSettings.Object);
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

            _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<IdentityUser>(), userUpsertion.Password)).
            Returns(Task.FromResult(IdentityResult.Failed(new IdentityError())));

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
        public void When_signup_method_is_called_with_create_async_user_method_returning_null_then_throw_server_error()
        {
            UserUpsertion userUpsertion = new UserUpsertion();
            userUpsertion.Name = "V";
            userUpsertion.MailId = "v@example.com";
            userUpsertion.Password = "123";

            _userManagerMock.Setup(x =>
            x.CreateAsync(It.IsAny<IdentityUser>(),
            "123")).Returns(Task.FromResult((IdentityResult)null));

            // Act
            var response = _loginController.Signup(userUpsertion);
            var objectResult = response.Result as ObjectResult;

            //// Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.InternalServerError);
        }

        [Test]
        public void When_signin_method_is_called_with_credentials_then_return_Ok_status_code()
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
        [TestCase("user@example.com", "")]
        [TestCase("", "123")]
        [TestCase("user@example", "123")]
        public void When_signin_method_is_called_with_wrong_format_then_return_bad_request_status_code(string emailId, string password)
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

        [Test]
        public void When_signin_method_is_called_with_unregistered_method_then_return_bad_request()
        {
            // Arrange
            LoginUserUpsertion loginUserUpsertion = new LoginUserUpsertion();
            loginUserUpsertion.MailId = "user@example.com";
            loginUserUpsertion.Password = "123";
            
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult((IdentityUser)null));

            // Act
            var response = _loginController.Signin(loginUserUpsertion);
            ObjectResult objectResult = response.Result as ObjectResult;

            // Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Test]
        public void When_signin_method_is_called_with_exception_throwing_method_then_return_bad_request()
        {
            // Arrange
            LoginUserUpsertion loginUserUpsertion = new LoginUserUpsertion();
            loginUserUpsertion.MailId = "user@example.com";
            loginUserUpsertion.Password = "123";

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _signInManagerMock.Setup(x =>
            x.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(),
                                       It.IsAny<string>(),
                                       It.IsAny<bool>())).
            Returns(Task.FromResult(SignInResult.Success));

            _signInManagerMock.Setup(x =>
            x.PasswordSignInAsync(It.IsAny<IdentityUser>(),
                                  It.IsAny<string>(),
                                  It.IsAny<bool>(),
                                  It.IsAny<bool>())).
            Throws(new Exception());

            // Act
            var response = _loginController.Signin(loginUserUpsertion);
            ObjectResult objectResult = response.Result as ObjectResult;

            // Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Test]
        public void When_signin_method_is_called_with_find_by_email_method_returning_null_then_return_bad_request_error()
        {
            // Arrange
            LoginUserUpsertion loginUserUpsertion = new LoginUserUpsertion();
            loginUserUpsertion.MailId = "user@example.com";
            loginUserUpsertion.Password = "123";

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult((IdentityUser)null));

            // Act
            var response = _loginController.Signin(loginUserUpsertion);
            ObjectResult objectResult = response.Result as ObjectResult;

            // Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Test]
        public void When_signin_method_with_any_other_exception_then_return_internal_server_error()
        {
            // Arrange
            LoginUserUpsertion loginUserUpsertion = new LoginUserUpsertion();
            loginUserUpsertion.MailId = "user@example.com";
            loginUserUpsertion.Password = "123";

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Throws(new Exception());

            // Act
            var response = _loginController.Signin(loginUserUpsertion);
            ObjectResult objectResult = response.Result as ObjectResult;

            // Assert
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.InternalServerError);
        }


        [Test]
        public void When_forgetpassword_method_is_called_with_credentials_then_return_true()
        {
            // Arrange
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>())).
            Returns(Task.FromResult("Token"));

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost");

            _loginController.Url = urlHelper.Object;

            _emailSender.Setup(x =>
            x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).
            Returns(Task.FromResult(true));

            // Act
            var response = _loginController.ForgetPassword("user@example.com");

            // Assert
            Assert.IsTrue(response.Result);

        }

        [Test]
        public void When_forgetpassword_method_is_called_with_unregistered_email_then_return_false()
        {
            // Arrange
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult((IdentityUser)null));

            // Act
            var response = _loginController.ForgetPassword("user@example.com");

            // Assert
            Assert.IsFalse(response.Result);
        }

        [Test]
        public void When_forgetpassword_method_is_called_with_send_email_throwing_exception_then_return_false()
        {
            // Arrange
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>())).
            Returns(Task.FromResult("Token"));

            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost");

            _loginController.Url = urlHelper.Object;

            _emailSender.Setup(x =>
            x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).
            Throws(new Exception());

            // Act
            var response = _loginController.ForgetPassword("user@example.com");

            // Assert
            Assert.False(response.Result);

        }

        [Test]
        public void When_resetpassword_method_is_called_with_email_and_token_then_return_redirect_result()
        {
            // Arrange
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.VerifyUserTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).
            Returns(Task.FromResult(true));

            // Act
            var response = _loginController.ResetPassword("user@example.com", "token");

            // Assert
            Assert.IsNotNull(response.Result);

        }

        [Test]
        public void When_resetpassword_method_is_called_with_unregistered_email_and_token_then_return_false()
        {
            // Arrange
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult((IdentityUser)null));

            // Act
            var response = _loginController.ResetPassword("user@example.com", "token");

            // Assert
            Assert.IsFalse(response.Result.Value);
        }

        [Test]
        public void When_resetpassword_method_is_called_with_email_and_invalid_token_then_return_false()
        {
            // Arrange
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.VerifyUserTokenAsync(It.IsAny<IdentityUser>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>())).
            Returns(Task.FromResult(false));


            // Act
            var response = _loginController.ResetPassword("user@example.com", "token");

            // Assert
            Assert.IsFalse(response.Result.Value);
        }

        [Test]
        public void When_resetpassword_method_is_called_with_exception_throwing_method_then_return_false()
        {
            // Arrange
            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.VerifyUserTokenAsync(It.IsAny<IdentityUser>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>())).
            Throws(new Exception());

            // Act
            var response = _loginController.ResetPassword("user@example.com", "token");

            // Assert
            Assert.IsFalse(response.Result.Value);

        }

        [Test]
        public void When_confirm_resetpassword_method_is_called_with_registered_email__and_valid_token_then_return_true()
        {
            // Arrange
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
            resetPasswordModel.Email = "user@example.com";
            resetPasswordModel.ConfirmPassword = "123";
            resetPasswordModel.Password = "123";
            resetPasswordModel.Token = "token";
            

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.ResetPasswordAsync(It.IsAny<IdentityUser>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>())).
            Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var response = _loginController.ConfirmResetPassword(resetPasswordModel);

            // Assert
            Assert.IsTrue(response.Result);
        }

        [Test]
        public void When_confirm_resetpassword_method_is_called_with_unregistered_email_then_return_false()
        {
            // Arrange
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
            resetPasswordModel.Email = "user@example.com";
            resetPasswordModel.ConfirmPassword = "123";
            resetPasswordModel.Password = "123";
            resetPasswordModel.Token = "token";

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult((IdentityUser)null));

            // Act
            var response = _loginController.ConfirmResetPassword(resetPasswordModel);

            // Assert
            Assert.IsFalse(response.Result);
        }

        [Test]
        public void When_confirm_resetpassword_method_is_called_with_registered_email_and_an_exception_throwing_method_then_return_true()
        {
            // Arrange
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
            resetPasswordModel.Email = "user@example.com";
            resetPasswordModel.ConfirmPassword = "123";
            resetPasswordModel.Password = "123";
            resetPasswordModel.Token = "token";


            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.ResetPasswordAsync(It.IsAny<IdentityUser>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>())).
            Throws(new Exception());

            // Act
            var response = _loginController.ConfirmResetPassword(resetPasswordModel);

            // Assert
            Assert.IsFalse(response.Result);

        }

        [Test]
        public void When_confirm_resetpassword_method_is_called_with_registered_email_and_fails_then_return_true()
        {
            // Arrange
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
            resetPasswordModel.Email = "user@example.com";
            resetPasswordModel.ConfirmPassword = "123";
            resetPasswordModel.Password = "123";
            resetPasswordModel.Token = "token";

            _userManagerMock.Setup(x =>
            x.FindByEmailAsync(It.IsAny<string>())).
            Returns(Task.FromResult(new IdentityUser()));

            _userManagerMock.Setup(x =>
            x.ResetPasswordAsync(It.IsAny<IdentityUser>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>())).
            Returns(Task.FromResult(IdentityResult.Failed(new IdentityError())));

            // Act
            var response = _loginController.ConfirmResetPassword(resetPasswordModel);

            // Assert
            Assert.IsFalse(response.Result);
        }


        [TearDown]
        public void TearDown()
        {
            _userManagerMock = null;
            _signInManagerMock = null;
            _emailSender = null;
            _loginController = null;

        }
    }
}