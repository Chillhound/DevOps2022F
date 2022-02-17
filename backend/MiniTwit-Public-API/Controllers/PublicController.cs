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
            //TODO: skal latest opdateres? I så fald: hvordan?
            var messages = _context.Messages.Where(m => m.Flagged == 0).Select(m => m).ToList();
            return messages;
        }

        [HttpGet]
        [Route("/latest")]
        public ActionResult GetLatest()
        {
            Console.WriteLine("latest endpoint er ramt med value:" + LatestResult.Latest);
            
            //return new OkObjectResult(LatestResult.Latest);
            return Ok(LatestResult.Latest);
        }

        [HttpGet]
        [Route("msgs/{username}")]
        public ActionResult<ICollection<Message>> GetMessagesUser(string username)
        {
            //Er det muligt at denne skal returnere 404 i tilfælde af ikke-eksisterende bruger?

            //han trækker antallet af besker ud af "args" - hvordan får vi det? er det ligesom /endpoint?limit=12?

            var user = _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            var messages = _context.Messages.Where(m => m.UserId == user.UserId).Where(m => m.Flagged == 0).Select(m => m).ToList();
            return messages; 
        }

        [HttpPost]
        [Route("msgs/{username}")]
        public IActionResult PostMessages(string username)
        {
            //Har taget udgangspunkt i at Helge IKKE validerer brugeren

            var content = Request.Query["content"].ToString();
            Console.WriteLine(content);

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

            return NoContent();
        }

        [HttpPost]
        [Route("/register")]
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


            _context.Add(user);
            _context.SaveChanges();

            

            //vi skal trække brugerinfo ud af request, ikke argument til funktionen 

            Console.WriteLine("LATEST: "+ LatestResult.Latest);

            return Ok();
        }

        [HttpGet]
        [Route("fllws/{username}")]
        public IActionResult FollowUser(string username, int no = 100)
        {
            var user = _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            if (user == null) return NotFound("yeeeeet");

            //hvordan fuck får man "args" ud af requesten? altså ligesom hans "no_followers"
            //args er vist bare det der bliver klasket bagpå? ellers så prøv med Request.Query

            return null;

        }

        [HttpPost]
        [Route("fllws/{username}")]
        public IActionResult ToggleFollowUser(string username)
        {

            //vi bør kunne gøre det på denne måde
            var data = Request.Query;
            if (data.ContainsKey("unfollow")) 
            {
                //smider exceptions pt. fordi db er tom
                //var userToBeUnfollowed = _context.Users.Where(u => u.UserName == data["unfollow"]);
                Console.WriteLine("Jeg skal unfollow "+data["unfollow"]);

                //resten af logikken
            }
            else if (data.ContainsKey("follow"))
            {
                //smider exceptions pt. fordi db er tom
                //var userToBeUnfollowed = _context.Users.Where(u => u.UserName == data["unfollow"]);
                Console.WriteLine("Jeg skal follow "+data["follow"]);

                //resten af logikken
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
