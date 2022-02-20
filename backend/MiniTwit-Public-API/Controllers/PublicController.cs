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
    //[Route("api/[controller]")]
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
        public ActionResult<ICollection<Message>> GetMessages()
        {
            LatestResult.Latest = int.Parse(Request.Query["latest"]);


            //TODO: skal latest opdateres? I så fald: hvordan?
            var messages = _context.Messages.Where(m => m.Flagged == 0).Select(m => new {content = m.Text, user = m.User.UserName}).ToList();
            return new JsonResult(messages);
        }

        [HttpGet]
        [Route("/latest")]
        public ActionResult GetLatest()
        {
            return Ok(new {latest = LatestResult.Latest}); //VICTORY! 
        }

        [HttpGet]
        [Route("msgs/{username}")]
        public ActionResult<ICollection<Message>> GetMessagesUser(string username)
        {
            //Er det muligt at denne skal returnere 404 i tilfælde af ikke-eksisterende bruger?

            //han trækker antallet af besker ud af "args" - hvordan får vi det? er det ligesom /endpoint?limit=12?

            LatestResult.Latest = int.Parse(Request.Query["latest"]);

            var user = _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            var messages = _context.Messages.Where(m => m.UserId == user.UserId).Where(m => m.Flagged == 0).Select(m => new {content = m.Text, user = m.User.UserName}).ToList();
            return new JsonResult(messages); 
        }

        [HttpPost]
        [Route("msgs/{username}")]
        public async Task<IActionResult> PostMessages(string username)
        {   
            //VIRKER NU, SLET ALT LORTET 

            var thething = await Request.ReadFromJsonAsync<TestDTO>();
            //Console.WriteLine("READFROMJSON :"+thething.content);
            //testen "test_create_msg" fejler fordi beskeden der lægges ind er null,
            //og det sker fordi jeg ikke kan finde ud af at få beskeden ud af requesten :) 
            //hvis man indsætter en random streng manuelt, så fungerer det fint aka logikken er god nok
            //PT. FEJLER DEN IKKE FORDI JEG HAR GIVET EN RANDOM STRENG MED

            //Har taget udgangspunkt i at Helge IKKE validerer brugeren

            //kan ikke fange content.. det lader til at det bliver sendt med i body, men aner ikke hvordan jeg får det ud når 
            //den også skal fange username via params... 
            //Console.WriteLine("POST ER RAMT BITCHES!!! - username: "+username);
            LatestResult.Latest = int.Parse(Request.Query["latest"]);

            //var hey = Request.BodyReader;
            //var cunt = hey.ReadAsync(); 


            //var content = Request.Query["content"];
            //Console.WriteLine(content); //content er ingenting... 

            var user = _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            if (user == null) return NotFound("yeeeeet"); //Helge kigger ikke på om brugeren findes, lol - måske skal vi heller ikke


            var message = new Message
            {
                User = user,
                UserId = user.UserId,
                Text = thething.content,
                PubDate = DateTime.UtcNow,
                Flagged = 0
            }; 
            _context.Messages.Add(message);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        [Route("/register")]
        [Produces("application/json")] //tror ikke det er nødvendigt.. 
        public IActionResult CreateUser([FromBody] CreateUserDTO userDTO)
        { 
            var lat = Request.Query["latest"];
            LatestResult.Latest = int.Parse(lat);

            if (userDTO == null) return BadRequest();
            string hashed = GetHash(userDTO.pwd);

            User user = new User
            {
                Email = userDTO.email,
                UserName = userDTO.username,
                PasswordHash = hashed

            };


            _context.Users.Add(user);
            _context.SaveChanges();
            
            //Console.WriteLine("LATEST: "+ LatestResult.Latest);
        
            return Ok(new {latest = lat});
        }

        [HttpGet]
        [Route("fllws/{username}")]
        public IActionResult FollowUser(string username, int no = 100)
        {
            Console.WriteLine("Get follow er ramt med username "+username);
            LatestResult.Latest = int.Parse(Request.Query["latest"]);

            var user = _context.Users.Include(u => u.Following).Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("useren er åbenbart null");
                return NotFound("yeeeeet");
            }
            var following = user.Following.Select(f => f.Whom.UserName);
            //Console.WriteLine("Størrelsen af "+username+" 's followers er: "+followers.Count());
            foreach(var follower in following)
            {
                Console.WriteLine("det er dem jeg følger: "+follower);
            }
            //TEST PASSER ! vi skal nok bare lige se på at regulere mængden der returneres

            //hvordan fuck får man "args" ud af requesten? altså ligesom hans "no_followers"
            //args er vist bare det der bliver klasket bagpå? ellers så prøv med Request.Query
            return Ok(new {latest = LatestResult.Latest, follows = following});
        }

        
        [HttpPost]
        [Route("fllws/{username}")]
        public async Task<IActionResult> ToggleFollowUser(string username)
        {
            //denne test passer pt. fordi den returnerer korrekte værdier, men 
            //logikken kører ikke, så derfor fejler tests på get-versionen

            Console.WriteLine("FLLWS POST ER RAMT!");

            LatestResult.Latest = int.Parse(Request.Query["latest"]);
            var fullAuthString = Request.Headers["Authorization"].ToString();
            var usableAuthString = fullAuthString.Substring(5);
            var decodedBytes = System.Convert.FromBase64String(usableAuthString);
            var decoded = System.Text.Encoding.UTF8.GetString(decodedBytes);
            var usernameAndPassword = decoded.Split(":");
            var requestingUsername = usernameAndPassword[0];
            var requestingPassword = usernameAndPassword[1];
            //Console.WriteLine(requestingUsername);
            //del del herover der dealer med auth er nok unødvendig 

            var requestingUser = _context.Users.Include(u => u.Following).Where(u => u.UserName == username).FirstOrDefault();
            if(requestingUser == null) Console.WriteLine("Jeg er null :((");
            //var req = JsonConvert.DeserializeObject<string>(Request);
            var thething = await Request.ReadFromJsonAsync<TestDTO>();

            //pt. kan den ikke fange unfollow/follow ligesom den ikke kan fange message
            //Console.WriteLine(data["unfollow"]);
            //har prøvet med Request.Form.Keys, Request.fuckdinmor og alt andet 
           
            Console.WriteLine("FOLLOW"+thething.follow);
            Console.WriteLine("UNFOLLOW"+thething.unfollow);

            if (thething.unfollow != null) 
            {
                Console.WriteLine("unfollow if er ramt - "+username+" skal unollow "+thething.unfollow);
                var unfollowName = thething.unfollow;
                var userToBeUnfollowed = _context.Users.Where(u => u.UserName == unfollowName).FirstOrDefault();
                if (userToBeUnfollowed == null) 
                {
                    return BadRequest();
                }

                var following = _context.Followers.Where(f => f.WhoId == requestingUser.UserId && f.WhomId == userToBeUnfollowed.UserId).FirstOrDefault();
                _context.Followers.Remove(following);
                _context.SaveChanges();
            }
            else if (thething.follow != null)
            {
                Console.WriteLine("follow if er ramt - "+username+" skal follow "+thething.follow);
                var followName = thething.follow;
                var userToBeFollowed = _context.Users.Include(u => u.Followers).Where(u => u.UserName == followName).FirstOrDefault();
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
                requestingUser.Following.Add(newFollowing);
                userToBeFollowed.Followers.Add(newFollowing);
                _context.Followers.Add(newFollowing);
                _context.SaveChanges();
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

        public class TestDTO
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
