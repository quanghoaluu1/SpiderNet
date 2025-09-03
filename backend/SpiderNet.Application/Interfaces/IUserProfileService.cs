using Microsoft.AspNetCore.Http;
using SpiderNet.Application.DTOs.User;
using SpiderNet.Domain.Common;

namespace SpiderNet.Application.Interfaces;

public interface IUserProfileService
{
    Task<Result<UserProfileDto>> GetUserProfileAsync(Guid userId, Guid? currentUserId = null);
    Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<Result<UserPrivateSettingDto>> GetPrivacySettingsAsync(Guid userId);
    Task<Result<UserPrivateSettingDto>> UpdatePrivacySettingsAsync(Guid userId, UpdatePrivacySettingsRequest request);
    Task<Result<string>> UploadAvatarAsync(Guid userId, IFormFile file);
    Task<Result<bool>> DeleteAvatarAsync(Guid userId);
}