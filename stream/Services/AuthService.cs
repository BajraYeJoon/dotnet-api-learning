using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using stream.Data;
using stream.Entities;
using stream.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace stream.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
                return null;

            //check for password
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
                PasswordVerificationResult.Failed)
            {
                return null;

            }
            return CreateToken(user);
        }

        public async Task<User?> RegisterAync(UserDto request)
        {
            if (await context.Users.AnyAsync(n => n.Username == request.Username))
                return null;


            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);
            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        private string CreateToken(User user)
        {
            //create claims for the token 
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Roles)
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
