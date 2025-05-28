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
        public async Task<IEnumerable<BlockDto>> GetAllBlocksAsync()
        {
            var blockEntities = await appDbContext.Blocks
              .Include(b => b.Floors)
              .Include(b => b.Houses)
              .ToListAsync();

            if (blockEntities.Count == 0)
            {
                return [];
            }

            var managerIds = blockEntities
                .Where(b => b.ManagerId.HasValue && b.ManagerId.Value != Guid.Empty)
                .Select(b => b.ManagerId.Value)
                .Distinct()
                .ToList();

            var managersDict = new Dictionary<Guid, string>();
            if (managerIds.Count > 0)
            {
                var managers = await userManager.Users
                .Where(u => managerIds.Contains(u.Id))
                .ToListAsync();

                managersDict = managers.ToDictionary(m => m.Id, m => m.UserName);
            }
            var blockDtos = blockEntities.Select(block =>
             {
                 string managerName = null;
                 if (block.ManagerId.HasValue && managersDict.TryGetValue(block.ManagerId.Value, out var name))
                 {
                     managerName = name;
                 }

                 return new BlockDto
                 {
                     Id = block.Id,
                     BlockName = block.BlockName,
                     PropertyType = block.PropertyType,
                     ManagerId = block.ManagerId == Guid.Empty ? null : block.ManagerId,
                     ManagerName = managerName,
                     // Assuming FloorDto and HouseDto are defined and BlockDto has these properties
                     Floors = block.Floors?.Select(f => new FloorDto { Id = f.Id, FloorNumber = f.FloorNumber, BlockId = f.BlockId /*, add other needed FloorDto properties */ }).ToList() ?? new List<FloorDto>(),
                     Houses = block.Houses?.Select(h => new HouseDto { Id = h.Id, HouseName = h.HouseName, BlockId = h.BlockId /*, add other needed HouseDto properties */ }).ToList() ?? new List<HouseDto>()
                 };
             }).ToList();


            return blockDtos;
        }
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
            var block = await appDbContext.Blocks

                .Include(b => b.Floors)
                .Include(b => b.Houses)
                .FirstOrDefaultAsync(b => b.Id == blockId);

            if (block == null)
            {
                return null;
            }

            string managerName = null;
            if (block.ManagerId.HasValue && block.ManagerId.Value != Guid.Empty)
            {
                var managerUser = await userManager.FindByIdAsync(block.ManagerId.Value.ToString());
                if (managerUser != null)
                {
                    managerName = managerUser.UserName;
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