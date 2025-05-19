using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController(ISuperAdminService superAdminService, IValidator<CreateManagerDto> managerValidator) : BaseApiController
    {
        [HttpPost("managers")]
        public async Task<IActionResult> CreateManager([FromBody] CreateManagerDto request)
        {
            var managerValidation = await managerValidator.ValidateAsync(request);
            if (!managerValidation.IsValid)
            {
                var errors = managerValidation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return ApiBadRequest<CreateManagerDto>(errors, "Validation Failed For Manager Creation");
            }

            try
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
            catch (Exception ex)
            {

                return ApiBadRequest<object>(
                    new Dictionary<string, string[]>
                    {
                        ["error"] = ["An unexpected error occurred while creating the manager"]
                    },
                    "Manager creation failed",
                    StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("managers")]
        public async Task<IActionResult> GetAllManagers()
        {
            try
            {
                var managers = await superAdminService.GetAllManagersAsync();

                return ApiOk(
                   managers,
                   "Managers retrieved successfully",
                   StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(
      new Dictionary<string, string[]>
      {
          ["error"] = ["An unexpected error occurred while retrieving managers"]
      },
      "Failed to retrieve managers",
      StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("SuperAdmin route is working!");
        }
    }



}