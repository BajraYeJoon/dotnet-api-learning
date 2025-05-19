namespace Core.DTOs
{
    public class CreateManagerDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<Guid>? AssignedBlocksIds { get; set; }
    }
}