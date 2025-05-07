using stream.Entities;
using stream.Models;

namespace stream.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(SignUpDto request);
        Task<RefreshTokenDto?> LoginAsync(LoginDto request);

        Task<RefreshTokenDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
