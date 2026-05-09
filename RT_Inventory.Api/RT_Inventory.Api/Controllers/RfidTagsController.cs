using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.DTOs.RfidTags;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/rfid-tags")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager},{RoleNames.Loader}")]
public class RfidTagsController(IRfidTagService rfidTagService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RfidTagResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tags = await rfidTagService.GetAllAsync();
        return Ok(tags);
    }

    [HttpPost("scan")]
    [ProducesResponseType(typeof(RfidTagResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Scan([FromBody] RfidTagScanRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await rfidTagService.ScanAsync(request, userId.Value);
        if (result.Tag is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to scan RFID tag." });
        }

        return CreatedAtAction(nameof(GetHistory), new { id = result.Tag.Id }, result.Tag);
    }

    [HttpPost("assign")]
    [ProducesResponseType(typeof(RfidTagResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign([FromBody] AssignRfidTagRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await rfidTagService.AssignAsync(request, userId.Value);
        if (result.Tag is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to assign RFID tag." });
        }

        return Ok(result.Tag);
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(RfidTagResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetRfidTagStatusRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await rfidTagService.SetStatusAsync(id, request.IsActive, userId.Value);
        if (result.Tag is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to update RFID tag status." });
        }

        return Ok(result.Tag);
    }

    [HttpGet("{id:int}/history")]
    [ProducesResponseType(typeof(IReadOnlyList<RfidTagHistoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var result = await rfidTagService.GetHistoryAsync(id);
        if (result.History is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to get RFID tag history." });
        }

        return Ok(result.History);
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
