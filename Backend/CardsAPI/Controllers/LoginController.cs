using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAPI.Models;
using CardsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardsAPI.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogin _loginService;

        public LoginController(ILogin loginService)
        {
            _loginService = loginService;
        }

        // POST api/values
        [HttpPost("Signup")]
        public ActionResult Post([FromBody]UserUpsertion user)
        {
            return Ok(_loginService.Signup(user));
        }
    }
}
