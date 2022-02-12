using DataAccess;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MiniTwit_API.Controllers
{
    public class MessageController : Controller
    {
        MiniTwitContext context;
        public MessageController(MiniTwitContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public ActionResult<List<Message>> GetUserById(int limit)
        {
            return context.Messages.OrderByDescending(x => x.PubDate).Take(limit).ToList();
        }
    }
}
