using Core.Entities;
using Core.DTOs;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(SignUpDto request);
        Task<RefreshTokenDto?> LoginAsync(LoginDto request);

        Task<RefreshTokenDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
