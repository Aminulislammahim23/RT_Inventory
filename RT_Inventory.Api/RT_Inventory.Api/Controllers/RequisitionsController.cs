using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.DTOs.Requisitions;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/requisitions")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.KnittingSupervisor},{RoleNames.UnitPlanner},{RoleNames.QualityOfficer},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager}")]
public class RequisitionsController(IRequisitionService requisitionService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RequisitionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRequisitionRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await requisitionService.CreateAsync(request, userId.Value);
        if (result.Requisition is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to create requisition." });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Requisition.Id }, result.Requisition);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RequisitionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var requisitions = await requisitionService.GetAllAsync();
        return Ok(requisitions);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RequisitionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var requisition = await requisitionService.GetByIdAsync(id);
        if (requisition is null)
        {
            return NotFound(new ApiMessageResponseDto { Message = "Requisition was not found." });
        }

        return Ok(requisition);
    }

    [HttpPost("{id:int}/picking-list")]
    [ProducesResponseType(typeof(PickingListResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePickingList(int id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await requisitionService.GeneratePickingListAsync(id, userId.Value);
        if (result.PickingList is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to generate picking list." });
        }

        return StatusCode(StatusCodes.Status201Created, result.PickingList);
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
