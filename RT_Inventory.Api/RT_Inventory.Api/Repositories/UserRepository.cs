using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id)
    {
        return dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == username);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await dbContext.Users.Include(x => x.Role).OrderBy(x => x.Username).ToListAsync();
    }

    public Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
    }

    public async Task AddAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
    }

    public Task SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}
