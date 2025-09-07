using AutoMapper;
using Mapster;
using SpiderNet.Application.DTOs;
using SpiderNet.Application.DTOs.Auth;
using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Application.DTOs.Post;
using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Application.DTOs.User;
using SpiderNet.Application.Extensions;
using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Mappings;

public class MappingConfig
{
    public static void Configure()
    {
       ConfigureUserMappings();
       ConfigurePostMappings();
       ConfigureCommentMappings();
       ConfigureReactionMappings();
    }

    private static void ConfigureUserMappings()
    {
         TypeAdapterConfig<User, UserDto>
            .NewConfig()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}".Trim());

         TypeAdapterConfig<RegisterRequest, User>
             .NewConfig()
             .Map(dest => dest.Email, src => src.Email != null ? src.Email.ToLower().Trim() : null)
             .Map(dest => dest.FirstName, src => src.FirstName != null ? src.FirstName.Trim() : null)
             .Map(dest => dest.LastName, src => src.LastName != null ? src.LastName.Trim() : null)
             .Map(dest => dest.AvatarUrl, src => src.AvatarUrl)
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
            .Map(dest => dest.DisplayName, src =>  src.FirstName)
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
    
     private static void ConfigurePostMappings()
    {
        // Post -> PostDto
        TypeAdapterConfig<Post, PostDto>
            .NewConfig()
            .Map(dest => dest.UserFullName, src => $"{src.User.FirstName} {src.User.LastName}".Trim())
            .Map(dest => dest.UserDisplayName, src => src.User.FirstName)
            .Map(dest => dest.UserAvatarUrl, src => src.User.AvatarUrl)
            .Map(dest => dest.TimeAgo, src => GetTimeAgo(src.CreatedAt))
            .Ignore(dest => dest.ReactionsSummary)
            .Ignore(dest => dest.CurrentUserReaction)
            .Ignore(dest => dest.IsOwnPost);

        // Post -> PostDetailDto
        TypeAdapterConfig<Post, PostDetailDto>
            .NewConfig()
            .Inherits<Post, PostDto>()
            .Ignore(dest => dest.Comments)
            .Ignore(dest => dest.RecentReactions);

        // CreatePostRequest -> Post (base mapping, need manual fields)
        TypeAdapterConfig<CreatePostRequest, Post>
            .NewConfig()
            .Map(dest => dest.Content, src => src.Content.Trim())
            .Map(dest => dest.Privacy, src => src.Privacy)
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.ImageUrl)
            .Ignore(dest => dest.VideoUrl)
            .Ignore(dest => dest.ImagePublicId)
            .Ignore(dest => dest.VideoPublicId);

        // UpdatePostRequest -> Post
        TypeAdapterConfig<UpdatePostRequest, Post>
            .NewConfig()
            .Map(dest => dest.Content, src => src.Content.Trim())
            .Map(dest => dest.Privacy, src => src.Privacy)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .IgnoreNonMapped(true);
    }

    private static void ConfigureCommentMappings()
    {
        // Comment -> CommentDto
        TypeAdapterConfig<Comment, CommentDto>
            .NewConfig()
            .Map(dest => dest.UserFullName, src => $"{src.User.FirstName} {src.User.LastName}".Trim())
            .Map(dest => dest.UserDisplayName, src =>  src.User.FirstName)
            .Map(dest => dest.UserAvatarUrl, src => src.User.AvatarUrl)
            .Map(dest => dest.TimeAgo, src => GetTimeAgo(src.CreatedAt))
            .Map(dest => dest.IsReply, src => src.ParentCommentId.HasValue)
            .Map(dest => dest.IsEdited, src => src.UpdatedAt > src.CreatedAt.AddMinutes(1))
            .Map(dest => dest.RepliesCount, src => src.Replies.Count(r => !r.IsDeleted))
            .Map(dest => dest.HasMedia, src => src.HasMedia)
            .Ignore(dest => dest.ReactionsSummary)
            .Ignore(dest => dest.CurrentUserReaction)
            .Ignore(dest => dest.IsOwnComment)
            .Ignore(dest => dest.Replies)
            .Ignore(dest => dest.HasMoreReplies);

        // CreateCommentRequest -> Comment
        TypeAdapterConfig<CreateCommentRequest, Comment>
            .NewConfig()
            .Map(dest => dest.Content, src => src.Content.Trim())
            .Map(dest => dest.ParentCommentId, src => src.ParentCommentId)
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .Ignore(dest => dest.PostId)
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.ImageUrl)
            .Ignore(dest => dest.VideoUrl)
            .Ignore(dest => dest.GifUrl)
            .Ignore(dest => dest.ImagePublicId)
            .Ignore(dest => dest.VideoPublicId)
            .Ignore(dest => dest.GifPublicId)
            .Ignore(dest => dest.MediaType);

        // UpdateCommentRequest -> Comment
        TypeAdapterConfig<UpdateCommentRequest, Comment>
            .NewConfig()
            .Map(dest => dest.Content, src => src.Content.Trim())
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .IgnoreNonMapped(true);
    }

    private static void ConfigureReactionMappings()
    {
        // Reaction -> ReactionDto
        TypeAdapterConfig<Reaction, ReactionDto>
            .NewConfig()
            .Map(dest => dest.UserFullName, src => $"{src.User.FirstName} {src.User.LastName}".Trim())
            .Map(dest => dest.UserAvatarUrl, src => src.User.AvatarUrl)
            .Map(dest => dest.TypeEmoji, src => src.Type.GetEmoji())
            .Map(dest => dest.TypeName, src => src.Type.GetDisplayName());

        // CommentReaction -> CommentReactionDto
        TypeAdapterConfig<CommentReaction, CommentReactionDto>
            .NewConfig()
            .Map(dest => dest.UserFullName, src => $"{src.User.FirstName} {src.User.LastName}".Trim())
            .Map(dest => dest.UserAvatarUrl, src => src.User.AvatarUrl)
            .Map(dest => dest.TypeEmoji, src => src.Type.GetEmoji())
            .Map(dest => dest.TypeName, src => src.Type.GetDisplayName());
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        else if (timeSpan.TotalHours < 1)
            return $"{(int)timeSpan.TotalMinutes}m";
        else if (timeSpan.TotalDays < 1)
            return $"{(int)timeSpan.TotalHours}h";
        else if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays}d";
        else
            return dateTime.ToString("MMM dd");
    }
}