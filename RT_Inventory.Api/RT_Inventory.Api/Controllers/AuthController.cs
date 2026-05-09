using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RT_Inventory.Api.DTOs.Auth;
using RT_Inventory.Api.DTOs.Common;
using RT_Inventory.Api.Interfaces;

namespace RT_Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await authService.LoginAsync(request);
        if (response is null)
        {
            return Unauthorized(new ApiMessageResponseDto { Message = "Invalid username or password." });
        }

        return Ok(response);
    }
}
