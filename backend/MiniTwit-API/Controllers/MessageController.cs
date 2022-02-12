using DataAccess;
using Domain.Models;
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

        [HttpPost]
        public ActionResult<List<Message>> Post(string messageText)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);

            Message message = new Message
            {
                PubDate = DateTime.Now,
                Flagged = 0,
                Author = context.Users.Find(userId),
                AuthorId = userId,
                Text = messageText
            };

            context.Messages.Add(message);
            context.SaveChanges();

            return Ok(message);
        }
    }
}
