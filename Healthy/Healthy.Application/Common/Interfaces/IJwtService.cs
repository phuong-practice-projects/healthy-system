using Healthy.Application.Common.Models;

namespace Healthy.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserDto user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    UserDto? GetUserFromToken(string token);
} 