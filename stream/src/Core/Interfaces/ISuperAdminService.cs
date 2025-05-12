using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces
{
    public interface ISuperAdminService
    {
        Task<User?> CreateManagerAsync(CreateManagerDto request);
        // Task<List<ManagerDto>> GetAllManagersAsync();
        // Task<bool> DeleteManagerAsync(Guid managerId);

        // Task<BlockDto> CreateBlockAsync(BlockDto request);

        // Task<List<BlockDto>> GetAllBlocksAsync();

    }
}




