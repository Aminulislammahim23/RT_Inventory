using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.DTOs.ReceiveSessions;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/receive-sessions")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager},{RoleNames.Loader}")]
public class ReceiveSessionsController(IReceiveSessionService receiveSessionService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ReceiveSessionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Start([FromBody] StartReceiveSessionRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await receiveSessionService.StartAsync(request, userId.Value);
        if (result.Session is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to start receive session." });
        }

        return StatusCode(StatusCodes.Status201Created, result.Session);
    }

    [HttpPost("{id:int}/scan")]
    [ProducesResponseType(typeof(ReceiveTagScanResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Scan(int id, [FromBody] ReceiveTagScanRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await receiveSessionService.ScanAsync(id, request, userId.Value);
        if (result.Scan is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to scan RFID tag." });
        }

        return Ok(result.Scan);
    }

    [HttpPost("{id:int}/deactivate")]
    [ProducesResponseType(typeof(ReceiveSessionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await receiveSessionService.DeactivateAsync(id);
        if (result.Session is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to deactivate receive session." });
        }

        return Ok(result.Session);
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
