using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Comment;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // User info
    public string UserFullName { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    
    // Interaction info
    public CommentReactionSummaryDto ReactionsSummary { get; set; } = new();
    public ReactionType? CurrentUserReaction { get; set; }
    public int RepliesCount { get; set; }
    
    // Meta info
    public string TimeAgo { get; set; } = string.Empty;
    public bool IsOwnComment { get; set; }
    public bool IsReply { get; set; }
    public bool IsEdited { get; set; }
    
    // Nested replies (only for top-level comments)
    public List<CommentDto> Replies { get; set; } = new();
    public bool HasMoreReplies { get; set; }
}