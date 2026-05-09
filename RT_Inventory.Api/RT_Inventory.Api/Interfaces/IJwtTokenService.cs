using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
