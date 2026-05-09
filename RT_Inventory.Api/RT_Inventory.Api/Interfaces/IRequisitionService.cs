using RT_Inventory.Api.DTOs.Requisitions;

namespace RT_Inventory.Api.Interfaces;

public interface IRequisitionService
{
    Task<(RequisitionResponseDto? Requisition, string? Error, int StatusCode)> CreateAsync(CreateRequisitionRequestDto request, int createdByUserId);
    Task<IReadOnlyList<RequisitionResponseDto>> GetAllAsync();
    Task<RequisitionResponseDto?> GetByIdAsync(int id);
    Task<(PickingListResponseDto? PickingList, string? Error, int StatusCode)> GeneratePickingListAsync(int requisitionId, int createdByUserId);
}
