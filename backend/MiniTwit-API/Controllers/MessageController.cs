using DataAccess;
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
        [HttpGet ("PublicTimeline")]
        public ActionResult<List<Message>> PublicTimeline(int limit)
        {
            return context.Messages.OrderByDescending(x => x.PubDate).Take(limit).ToList();
        }

        [Authorize]
        [HttpPost]
        public ActionResult<List<Message>> Post(string messageText)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);
            Message message = new Message
            {
                PubDate = DateTime.Now,
                Flagged = 0,
                AuthorId = userId,
                Text = messageText
            };

            var user = context.Users.Find(userId);
            if(user.Messages == null) { user.Messages = new List<Message>(); }
            user.Messages.Add(message);
            context.SaveChanges();

            return Ok(message);
        }
    }
}
