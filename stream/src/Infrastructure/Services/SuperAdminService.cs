using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class SuperAdminService(AppDbContext dbContext, UserManager<User> userManager, IEmailSenderService emailSenderService) : ISuperAdminService
    {
        public async Task<User?> CreateManagerAsync(CreateManagerDto request)
        {
            //firstly check if the manager already exists\
            if (await userManager.FindByEmailAsync(request.Username) != null ||
              await userManager.FindByEmailAsync(request.Email) != null)
            {
                return null;
            }

            //need transaction for complete operation
            using var createManagerTx = await dbContext.Database.BeginTransactionAsync();

            try
            {
                //creating the manager
                var manager = new User
                {
                    UserName = request.Username,
                    Email = request.Email,
                    EmailConfirmed = true,
                    Role = Roles.Manager,
                };

                var result = await userManager.CreateAsync(manager, request.Password);
                if (!result.Succeeded)
                {
                    return null;
                }

                // need to add role in identity
                await userManager.AddToRoleAsync(manager, Roles.Manager);

                //need to add manger to the block
                if (request.AssignedBlocksIds != null && request.AssignedBlocksIds.Any())
                {
                    var blocks = await dbContext.Blocks.Where(b => request.AssignedBlocksIds.Contains(b.Id)).ToListAsync();


                    if (blocks.Any())
                    {
                        foreach (var block in blocks)
                        {
                            block.ManagerId = manager.Id;

                        }
                        await dbContext.SaveChangesAsync();
                    }


                }

                //after the creation send mail
                await emailSenderService.SendEmailAsync(manager.Email, "Welcome to the AMS",
                $"Hi {manager.UserName}, <br/>" +
                $"Welcome to the AMS. <br/>" +
                $"Your account has been created successfully. <br/>" +
                $"You can login to the system using the following credentials: <br/>" +
                $"Username: {manager.UserName} <br/>" +
                $"Password: {request.Password} <br/>"
                );

                await createManagerTx.CommitAsync();
                return manager;
            }

            catch (Exception)
            {
                await createManagerTx.RollbackAsync();
                return null;
            }

        }


        // get all managers 
        public async Task<IEnumerable<ManagerResponseDto>> GetAllManagersAsync()
        {
            var managers = await userManager.GetUsersInRoleAsync(Roles.Manager);

            if (!managers.Any())
            {
                return Enumerable.Empty<ManagerResponseDto>();
            }

            var managerIds = managers.Select(m => m.Id).ToList();
            var blocksQuery = dbContext.Blocks
                .Where(b => managerIds.Contains(b.ManagerId));

            var blocksByManager = await blocksQuery
             .GroupBy(b => b.ManagerId)
             .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(b => new BlockDto

                {
                    Id = b.Id,
                    BlockName = b.BlockName,
                    PropertyType = b.PropertyType
                }).ToList()
             );


            return managers.Select(m => new ManagerResponseDto
            {
                Username = m.UserName,
                Email = m.Email,
                Role = m.Role,
                ManagedBlocks = blocksByManager.TryGetValue(m.Id, out var blocks) ? blocks : []
            });

        }

    }
}