// Controllers/Manager/ManagerController.cs
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Authorize(Roles = Roles.Manager)]
    [Route("api/managers")]
    [ApiController]
    public class ManagerController : BaseApiController
    {
        // Constructor with dependencies

        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            // Get users waiting for approval
            return ApiOk("List of pending users");
        }

        [HttpPost("approve-user/{id}")]
        public async Task<IActionResult> ApproveUser(Guid id)
        {
            // Approve a user
            return ApiOk("User approved");
        }
    }
}