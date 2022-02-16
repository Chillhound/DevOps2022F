﻿using Domain.DTO;
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

        int latest = 0;

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
        public LatestResult GetLatest()
        {
            return new LatestResult { latest = latest };
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

            //tror vi skal hente beskeden ud via noget af følgende: 
            //var maybeThis = HttpContext.Items["content"];
            //var orThis = Request.Body;
            //var orStream = new StreamReader(Request.Body);
            //var maybeBody = orStream.ReadToEnd();
            
            //nogle gode ideer?
            //det kan også være at det blot skal testes med postman eller lignende for rent faktisk at sende noget content

            //de her er måske også gode bud?
            // var halloooo = Request.Query["content"].ToString();
            // var halløj = HttpContext.Request.Query["content"].ToString();
            //HAR TESTET MED THUNDERCLIENT OG DET VIRKER !!! 

            //Console.WriteLine(halloooo);
            //Console.WriteLine(halløj);

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
        public IActionResult CreateUser(CreateUserDTO userDTO)
        {
            return null;
        }

        [HttpGet]
        [Route("fllws/{username}")]
        public IActionResult FollowUser(string username, int no = 100)
        {
            var user = _context.Users.Where(u => u.UserName == username).Select(u => u).FirstOrDefault();
            if (user == null) return NotFound("yeeeeet");

            //hvordan fuck får man "args" ud af requesten? altså ligesom hans "no_followers"

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

        public class LatestResult
        {
           public int latest { get; set; }
        }
    }
}
