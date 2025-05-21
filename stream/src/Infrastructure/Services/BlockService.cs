using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class BlockService(AppDbContext appDbContext, UserManager<User> userManager) : IBlockService
    {
        public async Task<BlockDto> CreateBlockAsync(CreateBlockDto createBlockDto)
        {
            if (createBlockDto.ManagerId.HasValue)
            {
                var manager = await userManager.FindByIdAsync(createBlockDto.ManagerId.Value.ToString());
                if (manager is null)
                    throw new ArgumentException("The manager for this id does not exist");

                var roles = await userManager.GetRolesAsync(manager);
                if (!roles.Contains(Roles.Manager))
                    throw new ArgumentException("This user is not a manager");
            }

            // check if the block name already exists
            var existingBlock = await appDbContext.Blocks.FirstOrDefaultAsync(b => b.BlockName.ToLower() == createBlockDto.BlockName.ToLower());
            if (existingBlock is not null)
            {
                throw new ArgumentException("Block name already exists");
            }

            var block = new Block
            {
                Id = Guid.NewGuid(),
                BlockName = createBlockDto.BlockName.ToLower(),
                PropertyType = createBlockDto.PropertyType,
                ManagerId = createBlockDto.ManagerId ?? Guid.Empty
            };

            await appDbContext.Blocks.AddAsync(block);
            await appDbContext.SaveChangesAsync();

            return await GetBlockDtoById(block.Id);
        }

        /// <summary>
        /// Helper method to get block DTO with manager information
        /// </summary>
        private async Task<BlockDto> GetBlockDtoById(Guid blockId)
        {
            var block = await appDbContext.Blocks.FindAsync(blockId);

            string? managerName = null;
            if (block is not null && block.ManagerId != Guid.Empty)
            {
                var manager = await userManager.FindByIdAsync(block.ManagerId.ToString());
                managerName = manager?.UserName;
            }

            return new BlockDto
            {
                Id = block.Id,
                BlockName = block.BlockName,
                PropertyType = block.PropertyType,
                ManagerId = block.ManagerId == Guid.Empty ? null : block.ManagerId,
                ManagerName = managerName
            };

        }
    }
}