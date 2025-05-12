using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Block> Blocks { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<House> Houses { get; set; }

        // public DbSet<User> Users { get; set; } 
        // DbSet<TUser> Users
        // DbSet<TRole> Roles
        // DbSet<IdentityUserClaim<TKey>> UserClaims
        // DbSet<IdentityUserRole<TKey>> UserRoles
        // DbSet<IdentityUserLogin<TKey>> UserLogins
        // DbSet<IdentityRoleClaim<TKey>> RoleClaims
        // DbSet<IdentityUserToken<TKey>> UserTokens
    }
}
