using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Data;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Repositories;

public class RfidTagRepository(ApplicationDbContext dbContext) : IRfidTagRepository
{
    public async Task<IReadOnlyList<RfidTag>> GetAllAsync()
    {
        return await dbContext.RfidTags
            .Include(x => x.AsnGrn)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public Task<RfidTag?> GetByIdAsync(int id)
    {
        return dbContext.RfidTags
            .Include(x => x.AsnGrn)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<RfidTag?> GetByTagValueAsync(string tagValue)
    {
        return dbContext.RfidTags
            .Include(x => x.AsnGrn)
            .FirstOrDefaultAsync(x => x.TagValue == tagValue);
    }

    public Task<RfidTag?> GetActiveByTagValueAsync(string tagValue)
    {
        return dbContext.RfidTags
            .Include(x => x.AsnGrn)
            .FirstOrDefaultAsync(x => x.TagValue == tagValue && x.IsActive);
    }

    public async Task<IReadOnlyList<RfidTagHistory>> GetHistoryAsync(int rfidTagId)
    {
        return await dbContext.RfidTagHistories
            .Include(x => x.RfidTag)
            .Include(x => x.PerformedByUser)
            .Where(x => x.RfidTagId == rfidTagId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(RfidTag rfidTag)
    {
        await dbContext.RfidTags.AddAsync(rfidTag);
    }

    public async Task AddHistoryAsync(RfidTagHistory history)
    {
        await dbContext.RfidTagHistories.AddAsync(history);
    }

    public Task SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}
