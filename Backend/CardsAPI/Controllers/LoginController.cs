using System;
using System.Threading.Tasks;
using CardsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Http;
using System.Net;
using CardsAPI.ExceptionHandling;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using CardsAPI.Helper;

namespace CardsAPI.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<AppSettings> _appSettings;

        public LoginController(UserManager<IdentityUser> userManager,
                               SignInManager<IdentityUser> signInManager,
                               IEmailSender emailSender, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> Signup([FromBody]UserUpsertion user)
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
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.StatusCode = HttpStatusCode.Created;
                    return Created(string.Empty, httpResponseMessage);
                }

                var errorStr = "";
                foreach(IdentityError error in result.Errors)
                {
                    errorStr += error.Description;
                }
                throw new LoginException(errorStr);
            }
            catch(LoginException ex) 
	        {
                return BadRequest(ex.Message);
		    }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Signin")]
        [AllowAnonymous]
        public async Task<object> Signin([FromBody] LoginUserUpsertion user)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(user.MailId);
                if (null == identityUser)
                {
                    throw new LoginException("No such user found");
                }
                var checkPasswordResult = await _signInManager.CheckPasswordSignInAsync(identityUser,user.Password,true);
                
                if (null != checkPasswordResult && checkPasswordResult.Succeeded)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.MailId, user.Password, true, false);
                    if (null != result && result.Succeeded)
                    {
                        User usr = new User();
                        usr.MailId = user.MailId;
                        usr.Name = usr.Name;

                        if (null != Authenticate(usr))
                        {
                            return Ok(usr);
                        }
                            
                    }
                }
                throw new LoginException("Incorrect Username / Password");
            }
            catch(LoginException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        private User Authenticate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Value.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.MailId)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = null;
            return user;
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<bool> ForgetPassword(string emailId)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(emailId);
                if (null == identityUser)
                {
                    return false;
                }

                var token = _userManager.GeneratePasswordResetTokenAsync(identityUser);
                string url = Url.Link("Reset", new { Action = "ResetPassword", Controller = "Login", emailId, token.Result });

                await _emailSender.SendEmailAsync(emailId, "Reset Password", url);

                return true;
            }
            catch 
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
                // redirect to reset password link if it is regardless
                // failure then show failure message in the resetpassword link.
                if (result) return Redirect("http://www.google.com");
                
                return result;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("ConfirmResetPassword")]
        public async Task<bool> ConfirmResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
                if (user == null) return false;
                var decodedToken = HttpUtility.UrlDecode(resetPasswordModel.Token);
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordModel.Password);

                if(result.Succeeded)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
