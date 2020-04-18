using System;
using System.Threading.Tasks;
using CardsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CardsAPI.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public LoginController(UserManager<IdentityUser> userManager,
                               SignInManager<IdentityUser> signInManager,
                               IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost("Signup")]
        public async Task<ActionResult<User>> Post([FromBody]UserUpsertion user)
        {
            try
            {
                var loginUser = new IdentityUser
                {
                    UserName = user.MailId,
                    Email = user.MailId,
                };
                var result = await _userManager.CreateAsync(loginUser, user.Password);
                if (result.Succeeded)
                {
                    User userModel = new User();
                    userModel.MailId = loginUser.Email;
                    return Ok(userModel);
                }
                //Throw errors Exception handling TODO
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        [HttpPost("Signin")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> Post([FromBody] LoginUserUpsertion user)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(user.MailId);
                if (null == identityUser)
                {
                    return false;
                }
                var result = await _signInManager.PasswordSignInAsync(user.MailId, user.Password, true, false);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> Post(string emailId)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(emailId);

                var token = _userManager.GeneratePasswordResetTokenAsync(identityUser);
                string url = Url.Link("Reset", new { Action = "ResetPassword", Controller = "Login", emailId, token.Result });

                await _emailSender.SendEmailAsync(emailId, "Reset Password", url);

                if (null == identityUser)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet("ResetPassword", Name = "Reset")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> ResetPassword(string emailId, string Result)
        {
            try
            {
                IdentityUser user = _userManager.FindByEmailAsync(emailId).Result;
                if (user == null) return false;
                var result = await _userManager.VerifyUserTokenAsync(user,
                                                                     _userManager.Options.Tokens.PasswordResetTokenProvider,
                                                                     "ResetPassword", Result);
                // if result is true then redirect to a link for resetting password and confirm password
                if (result) return Redirect("http://www.google.com");// redirect to reset password link if it is regardless
                // failure then show failure message in the resetpassword link.
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost("ConfirmResetPassword")]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
                if (user == null) return false;
                var decodedToken = HttpUtility.UrlDecode(resetPasswordModel.Token);
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordModel.Password);
                return result.Succeeded;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
