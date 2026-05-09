using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.DTOs.IssueSessions;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/issue-sessions")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager},{RoleNames.Loader}")]
public class IssueSessionsController(IIssueSessionService issueSessionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Start([FromBody] StartIssueSessionRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        var result = await issueSessionService.StartAsync(request, userId.Value);
        return result.Session is null
            ? StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to start issue session." })
            : StatusCode(StatusCodes.Status201Created, result.Session);
    }

    [HttpPost("{id:int}/scan")]
    public async Task<IActionResult> Scan(int id, [FromBody] IssueTagScanRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        var result = await issueSessionService.ScanAsync(id, request, userId.Value);
        return result.Scan is null
            ? StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to scan issue tag." })
            : Ok(result.Scan);
    }

    [HttpPost("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await issueSessionService.DeactivateAsync(id);
        return result.Session is null
            ? StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to deactivate issue session." })
            : Ok(result.Session);
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
