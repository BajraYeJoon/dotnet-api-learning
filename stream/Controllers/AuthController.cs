using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using stream.Entities;
using stream.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace stream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        public static User user = new();

        [HttpPost("sign-up")]
        public ActionResult<User> SignUp(UserDto request)
        {
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;
            return Ok(user);
        }

        [HttpPost("sign-in")]
        public ActionResult<User> 
            SignIn(UserDto request)
        {
            //check for the user first 
            if (user.Username != request.Username)
                return BadRequest("User not Found");

            //check for password
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
                PasswordVerificationResult.Failed)
            {
                return BadRequest("Please provide a password ");

            }

            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        { 
            //create claims for the token 
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username)
            };

            //generate key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            //now here's the things
            //create credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            //now create a token of jwt
            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            //return back the token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
