using DataAccess;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
        [HttpPost("Register")]
        public IActionResult Post([FromBody] CreateUserDTO userDTO)
        {
            if (userDTO == null) return BadRequest();
            string hashed = GetHash(userDTO.Password);

            User user = new User
            {
                Email = userDTO.Email,
                UserName = userDTO.UserName,
                PasswordHash = hashed
            };


            context.Add(user);
            context.SaveChanges();

            return NoContent();
        }
      

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO login)
        {
            if (login.Email == null || login.Password == null) return BadRequest();

            User user = context.Users.Where(x => x.Email == login.Email && x.PasswordHash == GetHash(login.Password) ).FirstOrDefault();

            if(user == null) return BadRequest();


          return Ok(login);
        }

        [HttpGet("Timeline")]
        public ActionResult<List<Message>> GetMessageTimeline(int id, int limit)
        {
            User user = context.Users.Find(id);

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

        //[HttpGet]
        //public ActionResult<List<Message>> GetMessages(int id)
        //{

        //    return context.Users.Find(id).Messages.ToList();
        //}

        private static string GetHash(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
            Console.WriteLine($"Hashed: {hashed}");
            return hashed;
        }
    }
}
