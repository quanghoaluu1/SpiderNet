using SpiderNet.Application.DTOs.Reaction;

namespace SpiderNet.Application.DTOs.Comment;

public class CommentReactionSummaryDto
{
    public int TotalCount { get; set; }
    public int LikesCount { get; set; }
    public int LovesCount { get; set; }
    public int HahaCount { get; set; }
    public int WowCount { get; set; }
    public int SadCount { get; set; }
    public int AngryCount { get; set; }
    
    // Most common reactions (max 3 for display)
    public List<ReactionTypeCount> TopReactions { get; set; } = new();
}