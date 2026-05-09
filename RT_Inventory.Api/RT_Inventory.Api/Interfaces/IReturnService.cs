using RT_Inventory.Api.DTOs.Returns;

namespace RT_Inventory.Api.Interfaces;

public interface IReturnService
{
    Task<(ReturnRequestResponseDto? ReturnRequest, string? Error, int StatusCode)> CreateAsync(CreateReturnRequestDto request, int createdByUserId);
    Task<IReadOnlyList<ReturnRequestResponseDto>> GetAllAsync();
    Task<(ReturnRequestResponseDto? ReturnRequest, string? Error, int StatusCode)> CreateReturnAsnGrnAsync(int id, CreateReturnAsnGrnRequestDto request, int createdByUserId);
}
