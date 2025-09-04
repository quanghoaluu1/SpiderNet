using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Post;

public class AddReactionRequest
{
    public ReactionType Type { get; set; }
}