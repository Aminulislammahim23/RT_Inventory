using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
