using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.DTOs.Returns;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/returns")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager},{RoleNames.KnittingSupervisor}")]
public class ReturnsController(IReturnService returnService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var returns = await returnService.GetAllAsync();
        return Ok(returns);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReturnRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        var result = await returnService.CreateAsync(request, userId.Value);
        return result.ReturnRequest is null
            ? StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to create return request." })
            : StatusCode(StatusCodes.Status201Created, result.ReturnRequest);
    }

    [HttpPost("{id:int}/asn-grn")]
    public async Task<IActionResult> CreateReturnAsnGrn(int id, [FromBody] CreateReturnAsnGrnRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        var result = await returnService.CreateReturnAsnGrnAsync(id, request, userId.Value);
        return result.ReturnRequest is null
            ? StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to create return ASN/GRN." })
            : StatusCode(StatusCodes.Status201Created, result.ReturnRequest);
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
