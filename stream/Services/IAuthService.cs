using stream.Entities;
using stream.Models;

namespace stream.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAync(UserDto request);
        Task<string?> LoginAsync(UserDto request);
    }
}
