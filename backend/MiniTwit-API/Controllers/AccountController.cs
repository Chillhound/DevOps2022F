using DataAccess;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MiniTwit_API.Service;
using System.Text;

namespace MiniTwit_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        MiniTwitContext context;
        JwtTokenHandler jwtTokenHandler;
        public AccountController(MiniTwitContext context)
        {
            this.context = context;
            jwtTokenHandler = new JwtTokenHandler();
        }

        //[HttpGet]
        //public async Task<ActionResult<User>> GetUserById(int id)
        //{
        //   return await context.Users.FindAsync(id);
        //}
        [HttpPost("Register")]
        public ActionResult<string> Post([FromBody] CreateUserDTO userDTO)
        {
            if (userDTO == null) return BadRequest();
            string hashed = GetHash(userDTO.pwd);

            User user = new User
            {
                Email = userDTO.email,
                UserName = userDTO.username,
                PasswordHash = hashed

            };


            context.Add(user);
            context.SaveChanges();

            return Ok(jwtTokenHandler.GenerateJwtToken(user));
        }

        [HttpPost("Login")]
        public ActionResult<string> Login([FromBody] LoginDTO login)
        {
            if (login.UserName == null || login.Password == null) return BadRequest("Email and password needs to be provided");

            User user = context.Users.Where(x => x.UserName == login.UserName && x.PasswordHash == GetHash(login.Password)).FirstOrDefault();

            if (user == null) return BadRequest("User not Found");


            return jwtTokenHandler.GenerateJwtToken(user);
        }

        private static string GetHash(string password)
        {
            var hmac = new HMACSHA512(Encoding.ASCII.GetBytes("secretkey"));

            return Encoding.ASCII.GetString(hmac.ComputeHash(Encoding.ASCII.GetBytes(password)));

        }
    }
}
