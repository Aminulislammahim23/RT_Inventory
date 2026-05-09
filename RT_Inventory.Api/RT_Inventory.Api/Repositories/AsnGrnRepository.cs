using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Repositories;

public class AsnGrnRepository(ApplicationDbContext dbContext) : IAsnGrnRepository
{
    public Task<AsnGrn?> GetByIdAsync(int id)
    {
        return dbContext.AsnGrns
            .Include(x => x.CreatedByUser)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<AsnGrn?> GetByAsnGrnNoAsync(string asnGrnNo)
    {
        return dbContext.AsnGrns
            .Include(x => x.CreatedByUser)
            .FirstOrDefaultAsync(x => x.AsnGrnNo == asnGrnNo);
    }

    public async Task<IReadOnlyList<AsnGrn>> GetAllAsync()
    {
        return await dbContext.AsnGrns
            .Include(x => x.CreatedByUser)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(AsnGrn asnGrn)
    {
        await dbContext.AsnGrns.AddAsync(asnGrn);
    }

    public Task SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}
