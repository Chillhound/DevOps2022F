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
using Prometheus;
using Serilog;

namespace MiniTwit_Public_API.Controllers
{
    [Route("/")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly MiniTwitContext _context;
        private static readonly Counter RequestLatest = Metrics.CreateCounter("Latest", "Latest man");
        private static readonly Counter RequestCountUsers = Metrics.CreateCounter("Num_users", "The total number of users created");

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
            var limit = 100;

            var messages = await _context.Messages.Where(m => m.Flagged == 0).OrderByDescending(x => x.PubDate).Select(m => new {content = m.Text, pub_date = m.PubDate, user = m.User.UserName}).Take(limit).ToListAsync();
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
            Log.Logger.Information($"messages request for the user {username}");
            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                RequestLatest.IncTo(tmp);
                LatestResult.Latest = tmp;
            }
            var user = await _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefaultAsync();

           
            if (user == null)
            {
                Log.Logger.Information($"The User couldnt be Retrived from DB");
                return BadRequest();
            }
            Log.Logger.Information($"Retrived the user {user.UserName} from DB");
            var messages = await _context.Messages.Where(m => m.UserId == user.UserId).Where(m => m.Flagged == 0).OrderByDescending(x => x.PubDate).Select(m => new {content = m.Text,pub_date = m.PubDate, user = m.User.UserName}).ToListAsync();
            return new JsonResult(messages); 
        }

        [HttpPost]
        [Route("msgs/{username}")]
        public async Task<IActionResult> PostMessages(string username)
        {
            Log.Logger.Information($"Post messages request reviced from user {username}");
            var data = await Request.ReadFromJsonAsync<apiDTO>();
            //Har taget udgangspunkt i at Helge IKKE validerer brugeren

            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                RequestLatest.IncTo(tmp);
                LatestResult.Latest = tmp;
            }

            var user = await _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefaultAsync();
            //if (user == null) return Forbid(); //Helge kigger ikke på om brugeren findes, lol - måske skal vi heller ikke
            Log.Logger.Information($"Retrived the user {user.UserName} from DB");

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
            Log.Logger.Information($"The message from {user.UserName} has been saved to db");
            return NoContent();
        }

        [HttpPost]
        [Route("/register")]
        [Produces("application/json")] //tror ikke det er nødvendigt.. 
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDTO)
        { 
            int tmp; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            Log.Logger.Information($"Create user request with username: {userDTO.username} ");
            if (hasLatest)
            {
                RequestLatest.IncTo(tmp);
                LatestResult.Latest = tmp;
            }

            if (userDTO == null) return BadRequest();
            string hashed = GetHash(userDTO.pwd);

            var alreadyExists = await _context.Users.Where(u => u.UserName == userDTO.username).FirstOrDefaultAsync();
            if (alreadyExists != null)
            {
                Log.Logger.Information($"The user alreadyexist in the db with: {userDTO.username} for the createuser request ");
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
            Log.Logger.Information($"The user {user.UserName} has been created, and saved to db");
            RequestCountUsers.Inc();


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
                RequestLatest.IncTo(tmp);
                LatestResult.Latest = tmp;
            }

            var user = await _context.Users.Include(u => u.Following).Where(u => u.UserName == username).Select(u => u).FirstOrDefaultAsync();
            if (user == null)
            {
                Log.Logger.Information($"The user with {username} was not found in db");
                return NotFound();
            }
            var following = await _context.Followers.Include(f => f.Whom).Where(f => f.WhoId == user.UserId).Select(f => f.Whom.UserName).ToListAsync();
            Log.Logger.Information($"Retrvied follows and returning list of follows");
            return Ok(new {follows = following});
        }

        
        [HttpPost]
        [Route("fllws/{username}")]
        public async Task<IActionResult> ToggleFollowUser(string username)
        {
            Log.Logger.Information($"Recived a follow request with username {username}");
            int tmp = 0; 
            var hasLatest = int.TryParse(Request.Query["latest"], out tmp);
            if(hasLatest)
            {
                RequestLatest.IncTo(tmp);
                LatestResult.Latest = tmp;
            }

            var requestingUser = await _context.Users.Include(u => u.Following).Where(u => u.UserName == username).FirstOrDefaultAsync();
           
            if(requestingUser == null)
            {
                Log.Logger.Information($"The requesting user for the follow request was not found ");
                return NotFound();
            }
            Log.Logger.Information($"The requesting user for the follow request was retrived from db with username: {requestingUser.UserName}");
            var data = await Request.ReadFromJsonAsync<apiDTO>();

            if (data.unfollow != null) 
            {
                Log.Logger.Information($"The follow request is a unfollow request");
                var unfollowName = data.unfollow;
                var userToBeUnfollowed = await _context.Users.Where(u => u.UserName == unfollowName).FirstOrDefaultAsync();
                if (userToBeUnfollowed == null) 
                {
                    Log.Logger.Information($"The user to be unfollowed was not found in the db {unfollowName}");
                    return NotFound();
                }

                var following = await _context.Followers.Where(f => f.WhoId == requestingUser.UserId && f.WhomId == userToBeUnfollowed.UserId).FirstOrDefaultAsync();
                if (following != null)
                {
                    
                    _context.Followers.Remove(following);
                    await _context.SaveChangesAsync();
                    Log.Logger.Information($"The follow relationship between {requestingUser.UserId} And {userToBeUnfollowed.UserId} has been removed");
                } 
                else
                {
                    Log.Logger.Information($"The follow relationship couldnt be found in db with {requestingUser.UserId} And {userToBeUnfollowed.UserId}");
                }
            }
            else
            {
                
                var followName = data.follow;
                Log.Logger.Information($"The follow request is a follow request to user: {followName}");
                var userToBeFollowed = await _context.Users.Include(u => u.Followers).Where(u => u.UserName == followName).FirstOrDefaultAsync();
                if (userToBeFollowed == null) 
                {
                    Log.Logger.Information($"The user to be followed was Not found in the db");
                    return NotFound();
                }
                Log.Logger.Information($"The user to be followed was found in db with the username {followName}");
                var exists = await _context.Followers.AnyAsync(e => e.WhoId == requestingUser.UserId && e.WhomId == userToBeFollowed.UserId);

                if (exists) 
                {
                    Log.Logger.Information($"the follow relationship allready exists");
                    return NoContent();
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
                await _context.SaveChangesAsync ();
                Log.Logger.Information($"The follow relationship has been saved to db");
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
