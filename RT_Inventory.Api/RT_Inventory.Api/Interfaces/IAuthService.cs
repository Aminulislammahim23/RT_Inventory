using RT_Inventory.Api.DTOs.Auth;
using RT_Inventory.Api.DTOs.Users;

namespace RT_Inventory.Api.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
    Task<(UserResponseDto? User, string? Error, int StatusCode)> RegisterAsync(RegisterUserRequestDto request);
    Task<IReadOnlyList<UserResponseDto>> GetUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto?> SetUserStatusAsync(int id, bool isActive);
}
