using System.Security.Claims;
using System.Threading.Tasks;
using CardsAPI.ExceptionHandling;
using CardsAPI.Models;
using CardsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CardsAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITopicServices _topicServices;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              ITopicServices topicServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _topicServices = topicServices;
        }

        [HttpPost("CreateTopic")]
        public async Task<IActionResult> CreateTopic([FromBody] TopicUpsertion topicUpsertion)
        {
            try
            {
                var isAuthenticated = User.Identity.IsAuthenticated;

                if (isAuthenticated)
                {
                    var currentLoggedUser = User.Identity.Name;
                    return Ok(await _topicServices.AddTopic(topicUpsertion,currentLoggedUser));
                }
                else
                {
                    return BadRequest("Please Login!!");
                }
            }
            catch(LoginException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetTopics")]
        public async Task<IActionResult> GetTopics()
        {
            try
            {
                var isAuthenticated = User.Identity.IsAuthenticated;

                if (isAuthenticated)
                {
                    var currentLoggedUser = User.Identity.Name;
                    return Ok(await _topicServices.GetTopics(currentLoggedUser));
                }
                else
                {
                    return BadRequest("Please Login!!");
                }
            }
            catch(LoginException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
