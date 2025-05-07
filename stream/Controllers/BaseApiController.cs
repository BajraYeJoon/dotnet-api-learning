using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using stream.Models;

namespace stream.Controllers
{
    public class BaseApiController: ControllerBase
    {
        protected IActionResult ApiOk<T>(T data, string message = "Success", int? statusCode = null)
        {
            return Ok(new ApiResponse<T>
            {

                Success = true,
                Message= message,
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
    }
}