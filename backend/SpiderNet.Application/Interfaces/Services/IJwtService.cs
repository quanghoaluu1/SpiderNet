using System.Security.Claims;
using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}