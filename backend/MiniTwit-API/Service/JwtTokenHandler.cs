using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniTwit_API.Service
{
    public class JwtTokenHandler
    {
        private readonly string secretKey = "this is a super secret key that needs to be in appsettings";
        public string GenerateJwtToken(User user)
        {
         
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()) }), // Add the user id into the token
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        //public int ValidateTokenAndGetUserID(HttpContext context)
        //{
        //    try
        //    {
        //       TokenHandler tokenHandler = new JwtSecurityTokenHandler();
        //       var key = Encoding.ASCII.GetBytes(secretKey);

        //        tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}
