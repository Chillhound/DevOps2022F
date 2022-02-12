using DataAccess;
using Domain.DTO;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MiniTwit_API.Service;

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
            string hashed = GetHash(userDTO.Password);

            User user = new User
            {
                Email = userDTO.Email,
                UserName = userDTO.UserName,
                PasswordHash = hashed
            };


            context.Add(user);
            context.SaveChanges();

            return Ok(jwtTokenHandler.GenerateJwtToken(user));
        }

        [HttpPost("Login")]
        public ActionResult<string> Login([FromBody] LoginDTO login)
        {
            if (login.Email == null || login.Password == null) return BadRequest("Email and password needs to be provided");

            User user = context.Users.Where(x => x.Email == login.Email && x.PasswordHash == GetHash(login.Password)).FirstOrDefault();

            if (user == null) return BadRequest("User not Found");


            return jwtTokenHandler.GenerateJwtToken(user);
        }

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
