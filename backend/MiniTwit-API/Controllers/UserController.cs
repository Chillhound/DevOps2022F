using DataAccess;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniTwit_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {

        MiniTwitContext context;

        public UserController(MiniTwitContext context)
        {
            this.context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<User>> GetUserById(int id)
        //{
        //   return await context.Users.FindAsync(id);
        //}
                 

        [HttpGet("Timeline")]
        public ActionResult<List<Message>> GetMessageTimeline(int id, int limit)
        {
            User user = context.Users.Find(id);
            if (user == null) return BadRequest();
            List<Message> messages = new List<Message>();

            messages.AddRange(user.Messages.Where(x => x.Flagged == 0).ToList());
    
            foreach (Follower x in user.Following)
            {
                User FollowingUser = context.Users.Find(x.WhoId);
                var mes = FollowingUser.Messages.Where(m => m.Flagged == 0).ToList();

                messages.AddRange(mes);
            }
            return messages.OrderByDescending(x => x.PubDate).Take(limit).ToList();
        }

        [Authorize]
        [HttpGet ("UserMessages")]
        public ActionResult<List<Message>> GetMessages(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);
            return context.Users.Find(id).Messages.ToList();
        }

    }
}
