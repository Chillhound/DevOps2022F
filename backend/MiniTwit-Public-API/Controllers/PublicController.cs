using Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwit_Public_API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {

        int latest = 0;

        [HttpGet ("msgs")]
        public IActionResult GetMessages()
        {
            return null;
        }

        [HttpGet("latest")]
        public LatestResult GetLatest()
        {
            return new LatestResult { latest = latest };
        }

        [HttpGet("{username}")]
        [Route("msgs/{username}")]
        public IActionResult GetMessagesUser(string username)
        {
            return null;
        }

        [HttpPost("{username}")]
        [Route("msgs/{username}")]
        public IActionResult PostMessages(string username)
        {
            return null;
        }

        [HttpPost("{username}")]
        [Route("register")]
        public IActionResult CreateUser(CreateUserDTO userDTO)
        {
            return null;
        }

        [HttpGet("{username}")]
        [Route("fllws/{username}")]
        public IActionResult FollowUser(string username, int no = 100)
        {
            return null;
        }

        [HttpPost("{username}")]
        [Route("fllws/{username}")]
        public IActionResult ToggleFollowUser(string username)
        {
            return null;
        }

        public class LatestResult
        {
           public int latest { get; set; }
        }
    }
}
