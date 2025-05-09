using Core.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stream.Core.DTOs;

namespace API.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected IActionResult ApiOk<T>(T data, string message = "Success", int? statusCode = null)
        {
            return Ok(new ApiResponse<T>
            {

                Message = message,
                StatusCode = statusCode ?? StatusCodes.Status200OK,
                Data = data,
            });
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