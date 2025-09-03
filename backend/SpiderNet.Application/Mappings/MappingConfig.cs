using AutoMapper;
using Mapster;
using SpiderNet.Application.DTOs;
using SpiderNet.Application.DTOs.Auth;
using SpiderNet.Application.DTOs.User;
using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Mappings;

public class MappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<User, UserDto>
            .NewConfig()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}".Trim());

        TypeAdapterConfig<RegisterRequest, User>
            .NewConfig()
            .Map(dest => dest.Email, src => src.Email.ToLower().Trim())
            .Map(dest => dest.Username, src => src.Username.ToLower().Trim())
            .Map(dest => dest.FirstName, src => src.FirstName.Trim())
            .Map(dest => dest.LastName, src => src.LastName.Trim())
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.IsActive, src => true)
            .Map(dest => dest.IsEmailConfirmed, src => false)
            .Ignore(dest => dest.PasswordHash);

        TypeAdapterConfig<User, AuthResponse>
            .NewConfig()
            .Map(dest => dest.User, src => src.Adapt<UserDto>())
            .Ignore(dest => dest.Token)
            .Ignore(dest => dest.RefreshToken)
            .Ignore(dest => dest.ExpiresAt);
        
        TypeAdapterConfig<User, UserProfileDto>
            .NewConfig()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}".Trim())
            .Map(dest => dest.DisplayName, src => 
                string.IsNullOrWhiteSpace(src.FirstName) ? src.Username : src.FirstName)
            .Map(dest => dest.MemberSince, src => src.CreatedAt.ToString("MMMM yyyy"))
            .Map(dest => dest.Age, src => src.DateOfBirth.HasValue ? 
                DateTime.UtcNow.Year - src.DateOfBirth.Value.Year : 0)
            .Map(dest => dest.IsOwnProfile, src => false);
        
        TypeAdapterConfig<UpdateProfileRequest, User>
            .NewConfig()
            .Map(dest => dest.FirstName, src => src.FirstName.Trim())
            .Map(dest => dest.LastName, src => src.LastName.Trim())
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .IgnoreNonMapped(true);
        TypeAdapterConfig<User, UserPrivateSettingDto>.NewConfig();
        TypeAdapterConfig<UpdatePrivacySettingsRequest, User>
            .NewConfig()
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .IgnoreNonMapped(true);
    }
}