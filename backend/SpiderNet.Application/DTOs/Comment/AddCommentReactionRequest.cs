using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Comment;

public class AddCommentReactionRequest
{
    public ReactionType Type { get; set; }
}