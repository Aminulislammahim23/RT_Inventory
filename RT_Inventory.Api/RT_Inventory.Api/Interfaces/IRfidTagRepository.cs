using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Interfaces;

public interface IRfidTagRepository
{
    Task<IReadOnlyList<RfidTag>> GetAllAsync();
    Task<RfidTag?> GetByIdAsync(int id);
    Task<RfidTag?> GetByTagValueAsync(string tagValue);
    Task<RfidTag?> GetActiveByTagValueAsync(string tagValue);
    Task<IReadOnlyList<RfidTagHistory>> GetHistoryAsync(int rfidTagId);
    Task AddAsync(RfidTag rfidTag);
    Task AddHistoryAsync(RfidTagHistory history);
    Task SaveChangesAsync();
}
