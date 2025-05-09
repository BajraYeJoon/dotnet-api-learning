using Core.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SuperAdmin
{
    [Authorize(Roles = Roles.SuperAdmin)]
    [Route("api/superadmin")]
    [ApiController]
    public class SuperAdminController : BaseApiController
    {
        // Constructor with dependencies

        [HttpPost("create-manager")]
        public async Task<IActionResult> CreateManager([FromBody] CreateManagerDto request)
        {
            // Logic to create a manager
            // This should create a user with Manager role and assign blocks

            return ApiOk("Manager created successfully");
        }

        [HttpGet("managers")]
        public async Task<IActionResult> GetManagers()
        {
            // Get all managers
            return ApiOk("List of managers");
        }
    }
}