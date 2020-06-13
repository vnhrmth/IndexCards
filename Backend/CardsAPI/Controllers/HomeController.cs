using System.Threading.Tasks;
using CardsAPI.Models;
using CardsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardsAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITopicServices _topicServices;

        public HomeController(ITopicServices topicServices)
        {
            _topicServices = topicServices;
        }

        [HttpPost("CreateTopic")]
        public async Task<IActionResult> CreateTopic([FromBody] TopicUpsertion topicUpsertion)
        {
            return Ok(await _topicServices.AddTopic(topicUpsertion));
        }
    }
}
