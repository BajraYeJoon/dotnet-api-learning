using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.SuperAdmin)]
    public class BlockController(IValidator<CreateBlockDto> createBlockValidator, IBlockService blockService) : BaseApiController
    {
        [HttpPost("create-block")]
        public async Task<IActionResult> CreateBlock([FromBody] CreateBlockDto request)
        {
            var blockCreateValidation = await createBlockValidator.ValidateAsync(request);
            if (!blockCreateValidation.IsValid)
            {
                var errors = blockCreateValidation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return ApiBadRequest<CreateBlockDto>(errors, "Validation Failed For Block Creation");
            }

            try
            {
                var block = await blockService.CreateBlockAsync(request);
                if (block is null)
                {
                    return ApiBadRequest<object>(
                        "Failed to create block",
                        "Block creation failed",
                        StatusCodes.Status400BadRequest);
                }

                return ApiOk(new { block.Id, block.BlockName },
                    "Block created successfully",
                    StatusCodes.Status201Created);
            }
            catch (ArgumentException ex)
            {
                return ApiBadRequest<object>(
                    new Dictionary<string, string[]>
                    {
                        ["error"] = [ex.Message]
                    },
                    "Block creation failed",
                    StatusCodes.Status400BadRequest);
            }
        }
    }
}