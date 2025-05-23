using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult ApiOk<T>(T data, string message = "Success", int? statusCode = null)
        {
            return Ok(new ApiResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = statusCode ?? StatusCodes.Status200OK,
                Data = data,
            });
        }
        protected static Dictionary<string, string[]> FormatValidationErrors(ValidationResult validationResult)
        {
            return validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }

        protected IActionResult ApiBadRequest<T>(object errors, string message = "BadRequest", int? statusCode = null)
        {
            return BadRequest(new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode ?? StatusCodes.Status400BadRequest,
                Errors = errors as Dictionary<string, string[]>,
            });
        }

        protected IActionResult ApiUnauthorized<T>(string message = "Unauthorized", object? errors = null)
        {
            return Unauthorized(new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors as Dictionary<string, string[]>,
                StatusCode = StatusCodes.Status401Unauthorized
            });
        }

        protected IActionResult ApiNotFound<T>(string message = "Not found", object? errors = null)
        {
            return NotFound(new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors as Dictionary<string, string[]>,
                StatusCode = StatusCodes.Status404NotFound
            });
        }
    }
}