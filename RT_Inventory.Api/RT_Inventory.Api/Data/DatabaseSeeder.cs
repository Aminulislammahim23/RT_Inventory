using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        await dbContext.Database.MigrateAsync();

        foreach (var roleName in RoleNames.All)
        {
            var roleExists = await dbContext.Roles.AnyAsync(x => x.Name == roleName);
            if (!roleExists)
            {
                dbContext.Roles.Add(new Role { Name = roleName });
            }
        }

        await dbContext.SaveChangesAsync();

        var adminExists = await dbContext.Users.AnyAsync(x => x.Username == "admin");
        if (adminExists)
        {
            return;
        }

        var adminRole = await dbContext.Roles.SingleAsync(x => x.Name == RoleNames.Admin);
        var admin = new User
        {
            Username = "admin",
            FullName = "System Administrator",
            Email = "admin@rt-inventory.local",
            IsActive = true,
            RoleId = adminRole.Id
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@12345");
        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}