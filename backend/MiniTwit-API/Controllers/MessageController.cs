﻿using DataAccess;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniTwit_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : Controller
    {
        MiniTwitContext context;
        public MessageController(MiniTwitContext context)
        {
            this.context = context;
        }
        [HttpGet("PublicTimeline")]
        public ActionResult<List<PublicMessageDTO>> PublicTimeline(int limit)
        {

            List<PublicMessageDTO> result = new List<PublicMessageDTO>();
            List<Message> messages = context.Messages.OrderByDescending(x => x.MessageId).Take(limit).ToList();

            foreach (Message message in messages)
            {
                User user = context.Users.Find(message.UserId);

                result.Add(new PublicMessageDTO
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    MessageId = message.MessageId,
                    UserId = user.UserId,
                    Flagged = message.Flagged,
                    PubDate = message.PubDate,
                    Text = message.Text
                });
            }

            return result;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<List<Message>> Post(string messageText)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);
            var user = context.Users.Find(userId);
            Message message = new Message
            {
                PubDate = DateTime.Now,
                Flagged = 0,
                UserId = userId,
                Text = messageText,
                User = user,
            };

            if (user.Messages == null) { user.Messages = new List<Message>(); }
            user.Messages.Add(message);
            context.SaveChanges();

            return Ok(message);
        }
    }
}
