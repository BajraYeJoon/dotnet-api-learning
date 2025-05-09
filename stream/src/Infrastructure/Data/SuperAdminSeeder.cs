using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public static class SuperAdminSeeder
    {
        //seed the initial data
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var scopeProvider = scope.ServiceProvider;

            try
            {
                var userManager = scopeProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scopeProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var logger = scopeProvider.GetRequiredService<ILogger<User>>();

                //create the superadmin role    
                if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.SuperAdmin));

                var superAdminEmail = "superadmin@bajra.com";
                var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

                if (superAdmin == null)
                {
                    logger.LogInformation("Creating superadmin user");
                    superAdmin = new User
                    {
                        UserName = "superadmin",
                        Email = superAdminEmail,
                        EmailConfirmed = true,
                        Role = Roles.SuperAdmin
                    };

                    var result = await userManager.CreateAsync(superAdmin, "superadmin123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
                        logger.LogInformation("Superadmin user created successfully");
                    }
                    else
                    {
                        var errors = string.Join(",", result.Errors.Select(e => e.Description));
                        logger.LogError("Failed to create superadmin user with errors: {errors}", errors);

                    }
                }
                else
                {
                    logger.LogInformation("Superadmin user already exists");
                }
            }
            catch (Exception e)
            {
                var logger = scopeProvider.GetRequiredService<ILogger>();
                logger.LogError(e, "An error occurred while seeding the database.");
            }
        }
    }
}
