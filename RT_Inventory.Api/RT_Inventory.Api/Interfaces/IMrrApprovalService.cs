using RT_Inventory.Api.DTOs.MrrApprovals;

namespace RT_Inventory.Api.Interfaces;

public interface IMrrApprovalService
{
    Task<(MrrApprovalResponseDto? Approval, string? Error, int StatusCode)> ApproveAsync(int asnGrnId, int approvedByUserId);
}
