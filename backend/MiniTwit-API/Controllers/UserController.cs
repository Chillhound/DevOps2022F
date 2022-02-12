using DataAccess;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MiniTwit_API.Controllers
{
    public class UserController : Controller
    {

        MiniTwitContext context;

        public UserController(MiniTwitContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
           return await context.Users.FindAsync(id);
        }

        [HttpGet]
        public ActionResult<List<Message>> GetMessageTimeline(int id, int limit)
        {
            User user = context.Users.Find(id);

            List<Message> messages = new List<Message>();

            messages.AddRange(user.Messages.Where(x => x.Flagged == 0).ToList());

            foreach (var x in user.Followers)
            {
                var mes = x.Messages.Where(m => m.Flagged == 0).ToList();

                messages.AddRange(mes);
            }

            return messages.OrderByDescending(x => x.PubDate).Take(limit).ToList();
        }

        [HttpGet]
        public ActionResult<List<Message>> GetMessages(int id)
        {
           
            return context.Users.Find(id).Messages.ToList();
        }


    }
}
