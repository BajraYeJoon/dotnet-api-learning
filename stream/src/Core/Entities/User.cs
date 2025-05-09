using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        // auth
        public string Role { get; set; } = Roles.User;
        public bool? IsApproved { get; set; } = false;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }

        // for manager
        public Guid? BlockId { get; set; }

        // for regular 
        public Guid? FlatId { get; set; }
        public Guid? HouseId { get; set; }
    }
}
