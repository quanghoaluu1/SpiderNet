using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Post;

public class PostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public PostPrivacy Privacy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // User info
    public string UserFullName { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    
    // Interaction info
    public ReactionSummaryDto ReactionsSummary { get; set; } = new();
    public ReactionType? CurrentUserReaction { get; set; }    public int CommentsCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public bool IsOwnPost { get; set; }
    
    // Time formatting
    public string TimeAgo { get; set; } = string.Empty;
}