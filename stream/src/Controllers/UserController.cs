using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.User
{
    [Authorize(Roles = Roles.User)]
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseApiController
    {
        // Constructor with dependencies

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Get user profile
            return ApiOk("User profile");
        }
    }
}