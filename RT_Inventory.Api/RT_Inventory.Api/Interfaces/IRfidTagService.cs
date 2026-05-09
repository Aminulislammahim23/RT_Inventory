using RT_Inventory.Api.DTOs.RfidTags;

namespace RT_Inventory.Api.Interfaces;

public interface IRfidTagService
{
    Task<IReadOnlyList<RfidTagResponseDto>> GetAllAsync();
    Task<(RfidTagResponseDto? Tag, string? Error, int StatusCode)> ScanAsync(RfidTagScanRequestDto request, int performedByUserId);
    Task<(RfidTagResponseDto? Tag, string? Error, int StatusCode)> AssignAsync(AssignRfidTagRequestDto request, int performedByUserId);
    Task<(RfidTagResponseDto? Tag, string? Error, int StatusCode)> SetStatusAsync(int id, bool isActive, int performedByUserId);
    Task<(IReadOnlyList<RfidTagHistoryResponseDto>? History, string? Error, int StatusCode)> GetHistoryAsync(int id);
}
