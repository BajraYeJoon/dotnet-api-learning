﻿using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Stream.Core.DTOs;
using Core.DTOs;
using Core.Interfaces;

namespace Controllers
{
    [EnableRateLimiting("anti-spam")]

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IValidator<SignUpDto> registerValidator, IValidator<LoginDto> loginValidator, UserManager<Core.Entities.User> userManager) : BaseApiController
    {
        [EnableRateLimiting("auth")]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto request)
        {
            var validationResult = await registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiBadRequest<UserSignUpResponseDto>(
                     FormatValidationErrors(validationResult),
                     "Validation failed",
                     StatusCodes.Status400BadRequest
                 );
            }

            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return ApiBadRequest<UserSignUpResponseDto>(
                    new Dictionary<string, string[]> { { "Email", new[] { "Email already exists" } } },
                    "Email already exists",
                    StatusCodes.Status409Conflict
                );
            }

            var responseAfterSignUp = new UserSignUpResponseDto
            {
                Id = user.Id,
                Email = user.Email,
            };
            return ApiOk(responseAfterSignUp, "User created successfully", StatusCodes.Status201Created);
        }
        [EnableRateLimiting("auth")]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(LoginDto request)
        {
            var validationResult = await loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiBadRequest<RefreshTokenDto>(
                     FormatValidationErrors(validationResult),
                     "Validation failed",
                     StatusCodes.Status400BadRequest
                 );
            }

            var user = await userManager.FindByEmailAsync(request.Email);
            // Fix the null check logic
            if (user is null)
            {
                return ApiBadRequest<RefreshTokenDto>(
                    new Dictionary<string, string[]> { { "Email", new[] { "Email not found" } } },
                    "Invalid credentials",
                    StatusCodes.Status401Unauthorized
                );
            }

            if (!user.EmailConfirmed)
            {
                return ApiBadRequest<RefreshTokenDto>(
                    new Dictionary<string, string[]> { { "Email", new[] { "Please verify your email before signing in" } } },
                    "Email not verified",
                    StatusCodes.Status403Forbidden
                );

            }

            var token = await authService.LoginAsync(request);
            if (token is null)
                return ApiBadRequest<RefreshTokenDto>("Invalid credentials", "Please check your email and password", StatusCodes.Status401Unauthorized);


            return ApiOk(token, "Login successful");
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiBadRequest<object>("User not found", "Please check your userId", StatusCodes.Status404NotFound);

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return ApiOk<object>("Email verified", "Email verified successfully", StatusCodes.Status200OK);

            return ApiBadRequest<object>("Email verification failed", "Please check your token", StatusCodes.Status400BadRequest);
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
