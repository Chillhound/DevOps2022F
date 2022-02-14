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
        public ActionResult<List<PublicMessageDTO>> GetMessageTimeline(int limit)
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
            return messages.OrderByDescending(x => x.PubDate).Select(message => new PublicMessageDTO
            {
                Email = message.User.Email,
                UserName = message.User.UserName,
                MessageId = message.MessageId,
                UserId = message.User.UserId,
                Flagged = message.Flagged,
                PubDate = message.PubDate,
                Text = message.Text
            }).Take(limit).ToList();
        }

        [Authorize]
        [HttpGet("UserMessages")]
        public ActionResult<UserMessagesDTO> GetMessages(int userId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            int id = int.Parse(identity.FindFirst("Id").Value);
            User user = context.Users.Find(userId);
            List<PublicMessageDTO> messages = context.Messages.Where(m => m.UserId == userId).Select(message => new PublicMessageDTO
            {
                UserName = user != null ? user.UserName : "",
                MessageId = message.MessageId,
                UserId = user != null ? user.UserId : 0,
                Email = user != null ? user.Email : "",
                Flagged = message.Flagged,
                PubDate = message.PubDate,
                Text = message.Text
            }).ToList();

            bool following = context.Followers.Any(r => r.WhoId == id && r.WhomId == userId);

            return new UserMessagesDTO
            {
                IsFollowing = following,
                messages = messages
            };
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
