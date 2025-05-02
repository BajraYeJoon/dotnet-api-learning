using stream.Data;
using stream.Entities;
using stream.Models;

namespace stream.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public Task<string?> LoginAsync(UserDto request)
        {
            throw new NotImplementedException();
        }

        public Task<User?> RegisterAync(UserDto request)
        {
            throw new NotImplementedException();
        }
    }
}
