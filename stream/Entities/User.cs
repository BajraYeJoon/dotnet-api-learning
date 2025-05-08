using Microsoft.AspNetCore.Identity;

namespace stream.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Roles { get; set; } =  string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
