using stream.Entities;
using stream.Models;

namespace stream.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<RefreshTokenDto?> LoginAsync(UserDto request);

        Task<RefreshTokenDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
