using AutoMapper;
using Mapster;
using Microsoft.AspNetCore.Identity;
using SpiderNet.Application.DTOs.Auth;
using SpiderNet.Application.Interfaces;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Domain.Common;
using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IPasswordValidationService _passwordValidationService;
    
    public AuthService(IJwtService jwtService, IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, IPasswordValidationService passwordValidationService)
    {
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _passwordValidationService = passwordValidationService;
    }
    
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetByUsernameOrEmailAsync(request.EmailOrUsername);
            if (user == null)
            {
                return Result<AuthResponse>.Failure("Invalid username or password.");
            }
            
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Result<AuthResponse>.Failure("Invalid username or password.");
            }
            
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var response = user.Adapt<AuthResponse>();
            response.Token = token;
            response.RefreshToken = refreshToken;
            response.ExpiresAt = DateTime.UtcNow.AddHours(2);
            
            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<AuthResponse>.Failure($"An error occurred during login: {ex.Message}");
        }
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (request.Password != request.ConfirmPassword)
            {
                return Result<AuthResponse>.Failure("Passwords do not match.");
            }
            
            var passwordValidation = _passwordValidationService.ValidatePassword(request.Password);
            if (!passwordValidation.IsValid)
            {
                return Result<AuthResponse>.Failure(passwordValidation.Errors);
            }

            if (await _unitOfWork.UserRepository.ExistByEmailAsync(request.Email))
            {
                return Result<AuthResponse>.Failure("Email already exists");
            }
            
            if (await _unitOfWork.UserRepository.ExistByUsernameAsync(request.Username))
            {
                return Result<AuthResponse>.Failure("Username already exists");
            }

            if (string.IsNullOrEmpty(request.Username))
            {
                request.Username = request.Email.Split('@')[0];
            }

            var user = request.Adapt<User>();
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            var createdUser = await _unitOfWork.UserRepository.CreateAsync(user);
            var response = createdUser.Adapt<AuthResponse>();
            response.Token = _jwtService.GenerateToken(createdUser);
            response.RefreshToken = _jwtService.GenerateRefreshToken();
            response.ExpiresAt = DateTime.UtcNow.AddHours(2);
            
            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<AuthResponse>.Failure($"An error occurred during registration: {ex.Message}");
        }
    }

    public Task<Result<AuthResponse>> RefreshTokenAsync(string token, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> LogoutAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}