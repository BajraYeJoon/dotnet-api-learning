namespace Core.DTOs
{
    public class ManagerResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<BlockDto> ManagedBlocks { get; set; } = [];
    }
}