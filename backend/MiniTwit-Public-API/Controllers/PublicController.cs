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
            var messages = _context.Messages.Where(m => m.Flagged == 0).Select(m => m).ToList();
            return messages;
        }

        [HttpGet]
        [Route("/latest")]
        public ActionResult GetLatest()
        {
            Console.WriteLine("latest endpoint er ramt med value:" + LatestResult.Latest);

            
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
            var messages = _context.Messages.Where(m => m.UserId == user.UserId).Where(m => m.Flagged == 0).Select(m => m).ToList();
            return messages; 
        }

        [HttpPost]
        [Route("msgs/{username}")]
        public IActionResult PostMessages(string username)
        {
            
            //testen "test_create_msg" fejler fordi beskeden der lægges ind er null,
            //og det sker fordi jeg ikke kan finde ud af at få beskeden ud af requesten :) 
            //hvis man indsætter en random streng manuelt, så fungerer det fint aka logikken er god nok

            //Har taget udgangspunkt i at Helge IKKE validerer brugeren

            //kan ikke fange content.. det lader til at det bliver sendt med i body, men aner ikke hvordan jeg får det ud når 
            //den også skal fange username via params... 
            Console.WriteLine("POST ER RAMT BITCHES!!! - username: ");
            Console.WriteLine(Request.Headers["content"]);
            LatestResult.Latest = int.Parse(Request.Query["latest"]);
            

            var content = Request.Query["content"];
            Console.WriteLine(content); //content er ingenting... 

            var user = _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            if (user == null) return NotFound("yeeeeet"); //Helge kigger ikke på om brugeren findes, lol - måske skal vi heller ikke


            var message = new Message
            {
                User = user,
                UserId = user.UserId,
                Text = content,
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
            
            Console.WriteLine("LATEST: "+ LatestResult.Latest);


        
            return Ok(new {latest = lat});
        }

        [HttpGet]
        [Route("fllws/{username}")]
        public IActionResult FollowUser(string username, int no = 100)
        {
            LatestResult.Latest = int.Parse(Request.Query["latest"]);

            var user = _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            if (user == null) return NotFound("yeeeeet");

            //hvordan fuck får man "args" ud af requesten? altså ligesom hans "no_followers"
            //args er vist bare det der bliver klasket bagpå? ellers så prøv med Request.Query

            return Ok(new {latest = LatestResult.Latest});
        }

        [HttpPost]
        [Route("fllws/{username}")]
        public IActionResult ToggleFollowUser(string username)
        {
            LatestResult.Latest = int.Parse(Request.Query["latest"]);
            var fullAuthString = Request.Headers["Authorization"].ToString();
            var usableAuthString = fullAuthString.Substring(5);
            var decodedBytes = System.Convert.FromBase64String(usableAuthString);
            var decoded = System.Text.Encoding.UTF8.GetString(decodedBytes);
            var usernameAndPassword = decoded.Split(":");
            var requestingUsername = usernameAndPassword[0];
            var requestingPassword = usernameAndPassword[1];

            var requestingUser = _context.Users.Where(u => u.UserName == requestingUsername).FirstOrDefault();


            //vi bør kunne gøre det på denne måde
            var data = Request.Query;
            if (data.ContainsKey("unfollow")) 
            {
                var userToBeUnfollowed = _context.Users.Where(u => u.UserName == data["unfollow"]).FirstOrDefault();
                if (userToBeUnfollowed == null) 
                {
                    return BadRequest();
                }

                var following = _context.Followers.Where(f => f.WhoId == requestingUser.UserId && f.WhomId == userToBeUnfollowed.UserId).FirstOrDefault();

                _context.Followers.Remove(following);
                _context.SaveChanges();

                Console.WriteLine("Jeg skal unfollow "+data["unfollow"]);
            }
            else if (data.ContainsKey("follow"))
            {
                var userToBeFollowed = _context.Users.Where(u => u.UserName == data["follow"]).FirstOrDefault();
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

                _context.Followers.Add(newFollowing);
                _context.SaveChanges();

                Console.WriteLine("Jeg skal follow "+data["follow"]);
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
    }
}
