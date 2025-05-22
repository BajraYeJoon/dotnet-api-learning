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


        public async Task<BlockDto> UpdateBlockAsync(Guid blockId, UpdateBlockDto updateBlock)
        {
            var block = await appDbContext.Blocks.FindAsync(blockId) ?? throw new ArgumentException("Block not found");

            if (updateBlock.ManagerId.HasValue)
            {
                if (updateBlock.ManagerId.Value != Guid.Empty)
                {
                    var manager = await userManager.FindByIdAsync(updateBlock.ManagerId.Value.ToString()) ?? throw new ArgumentException("The manager for this id does not exist");

                    var roles = await userManager.GetRolesAsync(manager);
                    if (!roles.Contains(Roles.Manager))
                    {
                        throw new ArgumentException("This user is not a manager");
                    }

                    block.ManagerId = updateBlock.ManagerId.Value;
                }
                else
                {
                    block.ManagerId = null;
                }


            }
            block.BlockName = updateBlock.BlockName;
            block.PropertyType = updateBlock.PropertyType;

            appDbContext.Blocks.Update(block);
            await appDbContext.SaveChangesAsync();

            return await GetBlockDtoById(block.Id);

        }
        /// <summary>
        /// Helper method to get block DTO with manager information
        /// </summary>
        private async Task<BlockDto> GetBlockDtoById(Guid blockId)
        {
            // Fetch the block entity from the database
            // Important: Include related entities if you want to display their data in the DTO
            var block = await appDbContext.Blocks
                // .Include(b => b.Manager) // If you had a direct navigation property to a User entity for manager
                .Include(b => b.Floors)  // If you want to include floor information
                .Include(b => b.Houses)  // If you want to include house information
                .FirstOrDefaultAsync(b => b.Id == blockId);

            if (block == null)
            {
                return null; // Or throw not found
            }

            string managerName = null;
            // If ManagerId is present, try to get the manager's name
            if (block.ManagerId.HasValue && block.ManagerId.Value != Guid.Empty)
            {
                var managerUser = await userManager.FindByIdAsync(block.ManagerId.Value.ToString());
                if (managerUser != null)
                {
                    managerName = managerUser.UserName; // Or FullName, etc.
                }
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