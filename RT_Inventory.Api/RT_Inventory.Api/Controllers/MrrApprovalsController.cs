using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.DTOs.MrrApprovals;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/mrr-approvals")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.StoreManager}")]
public class MrrApprovalsController(IMrrApprovalService mrrApprovalService) : ControllerBase
{
    [HttpPost("{asnGrnId:int}/approve")]
    [ProducesResponseType(typeof(MrrApprovalResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Approve(int asnGrnId)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await mrrApprovalService.ApproveAsync(asnGrnId, userId.Value);
        if (result.Approval is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to approve ASN/GRN." });
        }

        return Ok(result.Approval);
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
