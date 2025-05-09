namespace Core.DTOs
{
    public class UserDto : BaseUserDto
    {
        public bool IsApproved { get; set; }
        public ResidencyDto? Residence { get; set; }
    }
}
