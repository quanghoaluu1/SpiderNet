using SpiderNet.Application.DTOs.Auth;
using SpiderNet.Domain.Common;

namespace SpiderNet.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> RefreshTokenAsync(string token, string refreshToken);
    Task<Result<bool>> LogoutAsync(string refreshToken);
}