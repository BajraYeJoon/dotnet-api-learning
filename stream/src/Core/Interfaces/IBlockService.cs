using Core.DTOs;

namespace Core.Interfaces
{
    public interface IBlockService
    {
        Task<BlockDto> CreateBlockAsync(CreateBlockDto createBlockDto);
        Task<BlockDto> UpdateBlockAsync(Guid blockId, UpdateBlockDto updateBlockDto);
        // Task<bool> DeleteBlockAsync(Guid blockId);
        // Task<IEnumerable<BlockDto>> GetAllBlocksAsync();
        // Task<IEnumerable<BlockDto>> GetBlocksByManagerIdAsync(Guid managerId);
        // Task<BlockDto?> GetBlockByIdAsync(Guid blockId);
        // Task<BlockDto> AssignManagerToBlockAsync(Guid blockId, AssignManagerDto assignManagerDto);

    }
}