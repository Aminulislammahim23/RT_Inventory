using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.DTOs.Users;
using RT_Inventory.Api.Helpers;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.Admin)]
public class UsersController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request)
    {
        var result = await authService.RegisterAsync(request);
        if (result.User is null)
        {
            return StatusCode(result.StatusCode, new ApiMessageResponseDto { Message = result.Error ?? "Unable to register user." });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.User.Id }, result.User);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var users = await authService.GetUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await authService.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound(new ApiMessageResponseDto { Message = "User was not found." });
        }

        return Ok(user);
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetUserStatusRequestDto request)
    {
        var user = await authService.SetUserStatusAsync(id, request.IsActive);
        if (user is null)
        {
            return NotFound(new ApiMessageResponseDto { Message = "User was not found." });
        }

        return Ok(user);
    }
}
