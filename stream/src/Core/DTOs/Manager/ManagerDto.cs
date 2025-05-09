namespace Core.DTOs
{
    public class ManagerDto : BaseUserDto
    {
        public List<BlockDto> ManagedBlocks { get; set; } = [];
    }
}