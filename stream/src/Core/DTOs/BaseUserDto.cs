namespace Core.DTOs
{
    public abstract class BaseUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}