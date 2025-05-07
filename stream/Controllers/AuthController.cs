using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stream.Models;
using stream.Services;

namespace stream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IValidator<SignUpDto> registerValidator, IValidator<LoginDto> loginValidator) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<ActionResult<UserSignUpResponseDto>> SignUp(SignUpDto request)
        {
            var validationResult = await registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<UserSignUpResponseDto>
                {
                    Message = "Validation failed",
                    Success = false,
                    Errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    ),
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest(new ApiResponse<UserSignUpResponseDto>
                {
                    Message = "User already exists",
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Username", new[] { "User already exists" } }
                    }
                });
            }

            var responseAfterSignUp = new UserSignUpResponseDto
            {
                Id = user.Id,
                Username = user.Username
            };
            return Ok(new ApiResponse<UserSignUpResponseDto>
            {
                Message = "Registration successful",
                Success = true,
                Data = responseAfterSignUp,
                StatusCode = StatusCodes.Status201Created
            });
        }
        [HttpPost("sign-in")]
        public async Task<ActionResult<RefreshTokenDto>> SignIn(LoginDto request)
        {
            var validationResult = await loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<RefreshTokenDto>
                {
                    Message = "Validation failed",
                    Success = false,
                    Errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                });
            }

            var token = await authService.LoginAsync(request);
            if (token is null)
                return BadRequest(new ApiResponse<RefreshTokenDto>
                {
                    Message = "Invalid credentials",
                    Success = false,
                    StatusCode = StatusCodes.Status401Unauthorized,
                });

            return Ok(new ApiResponse<RefreshTokenDto>
            {
                Message = "Login successful",
                Success = true,
                Data = token,
                StatusCode = StatusCodes.Status200OK
            });
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
