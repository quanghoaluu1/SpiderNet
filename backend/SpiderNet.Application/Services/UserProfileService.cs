using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SpiderNet.Application.DTOs.User;
using SpiderNet.Application.Interfaces;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Domain.Common;
using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IPasswordValidationService _passwordValidationService;
    
    public UserProfileService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, IPasswordValidationService passwordValidationService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _passwordValidationService = passwordValidationService;
    }

    public async Task<Result<UserProfileDto>> GetUserProfileAsync(Guid userId, Guid? currentUserId = null)
    {
        var user = await _unitOfWork.UserRepository.GetUserProfileAsync(userId);
        if (user == null)
        {
            return Result<UserProfileDto>.Failure("User not found.");
        }
        var profile = user.Adapt<UserProfileDto>();
        profile.IsOwnProfile = currentUserId.HasValue && currentUserId.Value == user.Id;
        if (!profile.IsOwnProfile && user.IsPrivate)
        {
            profile.Email = user.ShowEmail ? profile.Email : string.Empty;
            profile.DateOfBirth = user is { ShowDateOfBirth: true, DateOfBirth: not null } ? user.DateOfBirth : null;
            profile.PhoneNumber = user.ShowPhoneNumber ? profile.PhoneNumber : string.Empty;
        }
        return Result<UserProfileDto>.Success(profile);
    }
    
    public async Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<UserProfileDto>.Failure("User not found");

        if (request.DateOfBirth.HasValue && request.DateOfBirth.Value > DateTime.UtcNow.AddYears(-13))
            return Result<UserProfileDto>.Failure("User must be at least 13 years old");

        request.Adapt(user);
        
        var updatedUser = await _unitOfWork.UserRepository.UpdateProfileAsync(user);
        var profile = updatedUser.Adapt<UserProfileDto>();
        profile.IsOwnProfile = true;

        return Result<UserProfileDto>.Success(profile);
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return Result<bool>.Failure("New passwords do not match.");
        }
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result<bool>.Failure("User not found.");
        }
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Result<bool>.Failure("Current password is incorrect.");
        }
        var passwordValidation = _passwordValidationService.ValidatePassword(request.NewPassword);
        if (!passwordValidation.IsValid)
        {
            return Result<bool>.Failure(passwordValidation.Errors);
        }
        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.UserRepository.UpdateAsync(user);
        return Result<bool>.Success(true);
    }

    public Task<Result<UserPrivateSettingDto>> GetPrivacySettingsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserPrivateSettingDto>> UpdatePrivacySettingsAsync(Guid userId, UpdatePrivacySettingsRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> UploadAvatarAsync(Guid userId, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAvatarAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}