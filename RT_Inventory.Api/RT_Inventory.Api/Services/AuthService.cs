using Microsoft.AspNetCore.Identity;
using RT_Inventory.Api.DTOs.Auth;
using RT_Inventory.Api.DTOs.Users;
using RT_Inventory.Api.Interfaces;
using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username.Trim());
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = jwtTokenService.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt,
            User = ToResponse(user)
        };
    }

    public async Task<(UserResponseDto? User, string? Error, int StatusCode)> RegisterAsync(RegisterUserRequestDto request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim().ToLowerInvariant();
        var roleName = request.Role.Trim();

        if (await userRepository.GetByUsernameAsync(username) is not null)
        {
            return (null, "Username already exists.", StatusCodes.Status409Conflict);
        }

        if (await userRepository.GetByEmailAsync(email) is not null)
        {
            return (null, "Email already exists.", StatusCodes.Status409Conflict);
        }

        var role = await userRepository.GetRoleByNameAsync(roleName);
        if (role is null)
        {
            return (null, "Role was not found.", StatusCodes.Status400BadRequest);
        }

        var user = new User
        {
            Username = username,
            FullName = request.FullName.Trim(),
            Email = email,
            RoleId = role.Id,
            Role = role,
            IsActive = request.IsActive
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        return (ToResponse(user), null, StatusCodes.Status201Created);
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(ToResponse).ToList();
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user is null ? null : ToResponse(user);
    }

    public async Task<UserResponseDto?> SetUserStatusAsync(int id, bool isActive)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        await userRepository.SaveChangesAsync();
        return ToResponse(user);
    }

    private static UserResponseDto ToResponse(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
