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

        [Authorize]
        [HttpGet("Me")]
        public async Task<ActionResult<User>> GetAuthenticatedUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int userId = int.Parse(identity.FindFirst("Id").Value);

            return await context.Users.FindAsync(userId);
        }

        [Authorize]
        [HttpGet("Timeline")]
        public ActionResult<List<Message>> GetMessageTimeline(int limit)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);

            User user = context.Users.Find(userId);
            if (user == null) return BadRequest();

            List<Message> messages = new List<Message>();


            if (user.Messages != null)
                messages.AddRange(context.Messages.Where(m => m.Flagged == 0 && m.UserId == userId).ToList());

            if (user.Following != null)
            {
                var FollowingUserList = context.Followers.Where(x => x.WhoId == userId).Select(x => x.Whom);

                foreach (User FollowingUser in FollowingUserList)
                {

                    if (FollowingUser.Messages != null)
                        messages.AddRange(context.Messages.Where(m => m.Flagged == 0 && m.UserId == FollowingUser.UserId).ToList());

                }
            }
            return messages.OrderByDescending(x => x.PubDate).Take(limit).ToList();
        }

        [Authorize]
        [HttpGet("UserMessages")]
        public ActionResult<List<Message>> GetMessages()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);
            return context.Messages.Where(m => m.UserId == userId).ToList();
        }

        [Authorize]
        [HttpGet("Follow")]
        public ActionResult<List<Message>> Follow(string username)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);
            var FollowUser = context.Users.Where(e => e.UserName == username).FirstOrDefault();

            var newRelation = new Follower
            {
                WhoId = userId,
                WhomId = FollowUser.UserId,
                Who = context.Users.Find(userId),
                Whom = FollowUser
            };
            var user = context.Users.Find(userId);
            if (user.Following == null) { user.Following = new List<Follower>(); }
            user.Following.Add(newRelation);
            context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpGet("Unfollow")]
        public ActionResult Unfollow(string username)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int userId = int.Parse(identity.FindFirst("Id").Value);
            var FollowingUser = context.Users.Where(e => e.UserName == username).Select(x => x.UserId).FirstOrDefault();

            var relation = context.Followers.Where(e => e.WhomId == FollowingUser && e.WhoId == userId).FirstOrDefault();
            context.Followers.Remove(relation);
            context.SaveChanges();
            return Ok();
        }

    }
}
