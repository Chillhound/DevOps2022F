using Domain.DTO;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace MiniTwit_Public_API.Controllers
{
    [Route("/")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly MiniTwitContext _context;

        public PublicController(MiniTwitContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/msgs")]
        public async Task<ActionResult<ICollection<Message>>> GetMessages()
        {
            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                LatestResult.Latest = tmp;
            }
            
            var messages = await _context.Messages.Where(m => m.Flagged == 0).Select(m => new {content = m.Text, user = m.User.UserName}).ToListAsync();
            return new JsonResult(messages);
        }

        [HttpGet]
        [Route("/latest")]
        public ActionResult GetLatest()
        {
            return Ok(new {latest = LatestResult.Latest});
        }

        [HttpGet]
        [Route("msgs/{username}")]
        public async Task<ActionResult<ICollection<Message>>> GetMessagesUser(string username)
        {
            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                LatestResult.Latest = tmp;
            }
            var user = await _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefaultAsync();
            if(user == null)
            {
                return BadRequest();
            }
            var messages = await _context.Messages.Where(m => m.UserId == user.UserId).Where(m => m.Flagged == 0).Select(m => new {content = m.Text, user = m.User.UserName}).ToListAsync();
            return new JsonResult(messages); 
        }

        [HttpPost]
        [Route("msgs/{username}")]
        public async Task<IActionResult> PostMessages(string username)
        {   
            var data = await Request.ReadFromJsonAsync<apiDTO>();
            //Har taget udgangspunkt i at Helge IKKE validerer brugeren

            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                LatestResult.Latest = tmp;
            }

            var user = await _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefaultAsync();
            if (user == null) return NotFound("yeeeeet"); //Helge kigger ikke på om brugeren findes, lol - måske skal vi heller ikke


            var message = new Message
            {
                User = user,
                UserId = user.UserId,
                Text = data.content,
                PubDate = DateTime.UtcNow,
                Flagged = 0
            }; 
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("/register")]
        [Produces("application/json")] //tror ikke det er nødvendigt.. 
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDTO)
        { 
            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                LatestResult.Latest = tmp;
            }

            if (userDTO == null) return BadRequest();
            string hashed = GetHash(userDTO.pwd);

            var alreadyExists = await _context.Users.Where(u => u.UserName == userDTO.username).FirstOrDefaultAsync();
            if (alreadyExists != null)
            {
                return BadRequest();
            }

            User user = new User
            {
                Email = userDTO.email,
                UserName = userDTO.username,
                PasswordHash = hashed
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpGet]
        [Route("fllws/{username}")]
        public async Task<IActionResult> FollowUser(string username, int no = 100)
        {
            //skal der ses på at bruge no til noget, så mængden der returneres kan justeres?
            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                LatestResult.Latest = tmp;
            }

            var user = await _context.Users.Include(u => u.Following).Where(u => u.UserName == username).Select(u => u).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("yeeeeet");
            }
            var following = await _context.Followers.Include(f => f.Whom).Where(f => f.WhoId == user.UserId).Select(f => f.Whom.UserName).ToListAsync();

            return Ok(new {latest = LatestResult.Latest, follows = following});
        }

        
        [HttpPost]
        [Route("fllws/{username}")]
        public async Task<IActionResult> ToggleFollowUser(string username)
        {
            int tmp = 0; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                LatestResult.Latest = tmp;
            }

            var requestingUser = await _context.Users.Include(u => u.Following).Where(u => u.UserName == username).FirstOrDefaultAsync();
            if(requestingUser == null)
            {
                return BadRequest("User does not exist");
            } 
            var data = await Request.ReadFromJsonAsync<apiDTO>();

            if (data.unfollow != null) 
            {
                var unfollowName = data.unfollow;
                var userToBeUnfollowed = await _context.Users.Where(u => u.UserName == unfollowName).FirstOrDefaultAsync();
                if (userToBeUnfollowed == null) 
                {
                    return BadRequest();
                }

                var following = await _context.Followers.Where(f => f.WhoId == requestingUser.UserId && f.WhomId == userToBeUnfollowed.UserId).FirstOrDefaultAsync();
                if (following != null)
                {
                    _context.Followers.Remove(following);
                    await _context.SaveChangesAsync();
                }
                
            }
            else if (data.follow != null)
            {
                var followName = data.follow;
                var userToBeFollowed = await _context.Users.Include(u => u.Followers).Where(u => u.UserName == followName).FirstOrDefaultAsync();
                if (userToBeFollowed == null) 
                {
                    return BadRequest();
                }

                var newFollowing = new Follower
                {
                    WhoId = requestingUser.UserId,
                    WhomId = userToBeFollowed.UserId,
                    Who = requestingUser,
                    Whom = userToBeFollowed
                };
                //requestingUser.Following.Add(newFollowing); //nok unødvendig
                //userToBeFollowed.Followers.Add(newFollowing); //nok unødvendig
                await _context.Followers.AddAsync(newFollowing);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        public static class LatestResult
        {
            public static int Latest;
        }

        private static string GetHash(string password)
        {
            var hmac = new HMACSHA512(Encoding.ASCII.GetBytes("secretkey"));

            return Encoding.ASCII.GetString(hmac.ComputeHash(Encoding.ASCII.GetBytes(password)));

        }

        private class apiDTO
        {
            [JsonPropertyName("content")]
            public string content {get; set;}

            [JsonPropertyName("follow")]
            public string follow {get; set;}

            [JsonPropertyName("unfollow")]
            public string unfollow {get; set;}
        }
    }
}
