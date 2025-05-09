using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration, UserManager<User> userManager, IEmailSenderService emailSenderService) : IAuthService
    {
        public async Task<RefreshTokenDto?> LoginAsync(LoginDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (user is null)
                return null;

            //check for password
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) ==
                PasswordVerificationResult.Failed)
            {
                return null;

            }

            //get that refresh token
            var response = await RefreshTokenDto(user);

            return response;
        }

        private async Task<RefreshTokenDto> RefreshTokenDto(User user)
        {
            var response = new RefreshTokenDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await SaveRefreshToken(user)
            };
            return response;
        }

        public async Task<User?> RegisterAsync(SignUpDto request)
        {

            //check if user already exists
            // if (await userManager.FindByNameAsync(request.Username) != null)
            // {
            //     return null;
            // }

            // init user
            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                Role = Roles.User
            };

            //now create 
            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return null;

            await userManager.AddToRoleAsync(user, Roles.User);
            // get the token
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // build link
            var confirmationLink = $"http://localhost:5251/api/auth/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            //send the mail
            await emailSenderService.SendEmailAsync(
              user.Email!,
              "Confirm your email",
              $"Please confirm your email by clicking on the link: <a href='{confirmationLink}'>Confirm</a>"
            );

            return user;



            // this is replaced with the aspnet.core identity

            // if (await context.Users.AnyAsync(n => n.UserName == request.Username))
            //     return null;


            // var user = new User();
            // var hashedPassword = new PasswordHasher<User>()
            //     .HashPassword(user, request.Password);
            // user.UserName = request.Username;
            // user.PasswordHash = hashedPassword;

            // context.Users.Add(user);
            // await context.SaveChangesAsync();

            // return user;
        }

        public async Task<RefreshTokenDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshToken(request.UserId, request.RefreshToken);
            if (user is null)
            {
                return null;
            }

            return await RefreshTokenDto(user);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        //this method will create a refresh token for the user
        private async Task<string> SaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<User?> ValidateRefreshToken(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private string CreateToken(User user)
        {
            //create claims for the token 
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role)
            };

            //generate key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!));

            //now here's the things
            //create credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            //now create a token of jwt
            var token = new JwtSecurityToken(
                issuer: configuration["AppSettings:Issuer"],
                audience: configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            //return back the token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
