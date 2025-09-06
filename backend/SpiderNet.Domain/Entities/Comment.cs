using SpiderNet.Domain.Enum;

namespace SpiderNet.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? GifUrl { get; set; }
    public string? ImagePublicId { get; set; } // For Cloudinary cleanup
    public string? VideoPublicId { get; set; }
    public string? GifPublicId { get; set; }
    public MediaType? MediaType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    
    public User User { get; set; } = new();
    public Post Post { get; set; } = new();
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    
    public int ReactionsCount => Reactions.Count;
    public int RepliesCount => Replies.Count(r => !r.IsDeleted);
    public bool IsReply => ParentCommentId.HasValue;
    public bool HasMedia => !string.IsNullOrEmpty(ImageUrl) || !string.IsNullOrEmpty(VideoUrl) || !string.IsNullOrEmpty(GifUrl);
}