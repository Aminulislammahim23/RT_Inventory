using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.AsnGrns;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/asn-grns")]
[Authorize(Roles = $"{RoleNames.Admin},{RoleNames.SCM},{RoleNames.StoreOfficer},{RoleNames.StoreSupervisor},{RoleNames.StoreManager}")]
public class AsnGrnsController(IAsnGrnService asnGrnService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(AsnGrnResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAsnGrnRequestDto request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Authenticated user id was not found in token." });
        }

        var result = await asnGrnService.CreateAsync(request, userId.Value);
        if (result.AsnGrn is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to create ASN/GRN." });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.AsnGrn.Id }, result.AsnGrn);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AsnGrnResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var asnGrns = await asnGrnService.GetAllAsync();
        return Ok(asnGrns);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AsnGrnResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var asnGrn = await asnGrnService.GetByIdAsync(id);
        if (asnGrn is null)
        {
            return NotFound(new ApiMessageResponseDto { Message = "ASN/GRN was not found." });
        }

        return Ok(asnGrn);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AsnGrnResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAsnGrnRequestDto request)
    {
        var result = await asnGrnService.UpdateAsync(id, request);
        if (result.AsnGrn is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to update ASN/GRN." });
        }

        return Ok(result.AsnGrn);
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
