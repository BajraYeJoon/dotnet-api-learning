using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using Microsoft.AspNetCore.RateLimiting;

namespace Controllers
{
    [EnableRateLimiting("auth")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.SuperAdmin)]
    public class BlockController(IBlockService _blockService, IValidator<CreateBlockDto> _createBlockValidator, IValidator<UpdateBlockDto> _updateBlockValidator) : BaseApiController
    {

        [HttpGet("get-all-blocks")]
        public async Task<IActionResult> GetAllBlocks()
        {
            try
            {
                var blocks = await _blockService.GetAllBlocksAsync();
                if (blocks is null || !blocks.Any())
                {
                    return ApiBadRequest<object>(null, "No blocks found", StatusCodes.Status204NoContent);
                }

                return ApiOk(blocks, "Blocks retrieved successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return ApiBadRequest<object>(null, "An error occurred while retrieving blocks.", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("create-block")]
        public async Task<IActionResult> CreateBlock([FromBody] CreateBlockDto request)
        {
            var validationResult = await _createBlockValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiBadRequest<object>(FormatValidationErrors(validationResult), "Validation Failed For Block Creation");
            }

            try
            {
                var block = await _blockService.CreateBlockAsync(request);
                if (block is null)
                {
                    return ApiBadRequest<object>(
                         new Dictionary<string, string[]>(),
                        "Failed to create block due to a business rule violation or internal issue.",
                        StatusCodes.Status400BadRequest);
                }

                return ApiOk(block,
                    "Block created successfully",
                    StatusCodes.Status201Created);
            }
            catch (ArgumentException ex)
            {
                return ApiBadRequest<object>(
                    null,
                    ex.Message,
                    StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the block.",
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPut("update-block/{blockId}")]
        public async Task<IActionResult> UpdateBlock(Guid blockId, [FromBody] UpdateBlockDto request)
        {
            ValidationResult validationResult = await _updateBlockValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return ApiBadRequest<object>(FormatValidationErrors(validationResult), "Validation Failed For Block Update");
            }

            try
            {
                var updatedBlock = await _blockService.UpdateBlockAsync(blockId, request);
                if (updatedBlock is null)
                {
                    return ApiBadRequest<object>(
                        "Block update failed. It might not exist or a business rule was violated.",
                        "Failed to update block. It might not exist or a business rule was violated.",
                        StatusCodes.Status400BadRequest);
                }

                return ApiOk(updatedBlock,
                    "Block updated successfully",
                    StatusCodes.Status200OK);
            }
            catch (ArgumentException ex)
            {
                return ApiBadRequest<object>(
                    null,
                    ex.Message,
                    StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the block.",
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}