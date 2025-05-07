using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stream.Entities;
using stream.Models;
using stream.Services;

namespace stream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("sign-up")]
        public async Task<ActionResult<ApiResponse<UserSignUpResponseDto>>> SignUp(UserDto request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<UserSignUpResponseDto>
                {
                    Message = "Validation failed",
                    Success = false
                });
            }

            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists");
            }

            var responseAfterSignUp = new UserSignUpResponseDto
            {
                Id = user.Id,
                Username = user.Username
            };
            return Ok(responseAfterSignUp);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<RefreshTokenDto>> 
            SignIn(UserDto request)
        {
            var token = await authService.LoginAsync(request);
            if(token is null)
                return BadRequest("Invalid credentials");

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var token = await authService.RefreshTokenAsync(request);
            if (token is null || token.AccessToken is null || token.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }

            return Ok(token);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedEndpoint()
        {
            return Ok("Authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok("Hmm you are an admin");
        }


    }
}
