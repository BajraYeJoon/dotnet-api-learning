using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController(ISuperAdminService superAdminService) : BaseApiController
    {
        [HttpPost("managers")]
        public async Task<IActionResult> CreateManager([FromBody] CreateManagerDto request)
        {
            var manager = await superAdminService.CreateManagerAsync(request);
            if (manager == null)
                return ApiBadRequest<object>(
                    "Failed to create manager",
                    "Manager creation failed",
                    StatusCodes.Status400BadRequest);

            return ApiOk(
                new { Id = manager.Id, Username = manager.UserName },
                "Manager created successfully",
                StatusCodes.Status201Created);
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("SuperAdmin route is working!");
        }
    }
}