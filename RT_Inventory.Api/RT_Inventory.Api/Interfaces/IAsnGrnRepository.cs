using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Interfaces;

public interface IAsnGrnRepository
{
    Task<AsnGrn?> GetByIdAsync(int id);
    Task<AsnGrn?> GetByAsnGrnNoAsync(string asnGrnNo);
    Task<IReadOnlyList<AsnGrn>> GetAllAsync();
    Task AddAsync(AsnGrn asnGrn);
    Task SaveChangesAsync();
}
