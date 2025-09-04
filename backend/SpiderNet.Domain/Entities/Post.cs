using SpiderNet.Domain.Enum;

namespace SpiderNet.Domain.Entities;

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? ImagePublicId { get; set; }
    public string? VideoPublicId { get; set; }
    public PostPrivacy Privacy { get; set; } = PostPrivacy.Public;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public int ReactionsCount => Reactions.Count;
    public int CommentsCount => Comments.Count;
    public int LikesCount => Reactions.Count(r => r.Type == ReactionType.Like);
    public int LovesCount => Reactions.Count(r => r.Type == ReactionType.Love);
    public int HahaCount => Reactions.Count(r => r.Type == ReactionType.Haha);
    public int WowCount => Reactions.Count(r => r.Type == ReactionType.Wow);
    public int SadCount => Reactions.Count(r => r.Type == ReactionType.Sad);
    public int AngryCount => Reactions.Count(r => r.Type == ReactionType.Angry);
}