using RT_Inventory.Api.DTOs.AsnGrns;

namespace RT_Inventory.Api.Interfaces;

public interface IAsnGrnService
{
    Task<(AsnGrnResponseDto? AsnGrn, string? Error, int StatusCode)> CreateAsync(CreateAsnGrnRequestDto request, int createdByUserId);
    Task<IReadOnlyList<AsnGrnResponseDto>> GetAllAsync();
    Task<AsnGrnResponseDto?> GetByIdAsync(int id);
    Task<(AsnGrnResponseDto? AsnGrn, string? Error, int StatusCode)> UpdateAsync(int id, UpdateAsnGrnRequestDto request);
}
