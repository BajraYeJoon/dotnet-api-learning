namespace Core.DTOs
{
    public class ApproveUserDto
    {
        public Guid UserId { get; set; }
        public Guid UnitId { get; set; }

        public bool IsApproved { get; set; } = true;
    }
}