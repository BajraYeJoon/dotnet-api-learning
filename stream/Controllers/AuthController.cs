using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stream.Models;
using stream.Services;

namespace stream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IValidator<SignUpDto> registerValidator, IValidator<LoginDto> loginValidator) : BaseApiController
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto request)
        {
            var validationResult = await registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return ApiBadRequest<UserSignUpResponseDto>(errors, "Validation failed", StatusCodes.Status400BadRequest);
            }

            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return ApiBadRequest<UserSignUpResponseDto>(
                    new Dictionary<string, string[]> { { "Username", new[] { "User already exists" } } },
                    "User already exists",
                    StatusCodes.Status409Conflict
                );
            }

            var responseAfterSignUp = new UserSignUpResponseDto
            {
                Id = user.Id,
                Username = user.Username
            };
            return ApiOk(responseAfterSignUp, "User created successfully", StatusCodes.Status201Created);
        }
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(LoginDto request)
        {
            var validationResult = await loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return ApiBadRequest<RefreshTokenDto>(errors, "Validation failed", StatusCodes.Status400BadRequest);
            }

            var token = await authService.LoginAsync(request);
            if (token is null)
                return ApiBadRequest<RefreshTokenDto>("Invalid credentials", "Please check your username and password", StatusCodes.Status401Unauthorized);

            return ApiOk(token, "Login successful");
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var token = await authService.RefreshTokenAsync(request);
            if (token is null || token.AccessToken is null || token.RefreshToken is null)
            {
                return ApiUnauthorized<RefreshTokenDto>("Invalid refresh token");
            }

            return ApiOk(token, "Token Refreshed");
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedEndpoint()
        {
            return ApiOk("Authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminEndpoint()
        {
            return ApiOk("Hmm you are an admin");
        }


    }
}
