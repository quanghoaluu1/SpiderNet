using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Reaction;

public class ReactionTypeCount
{
    public ReactionType Type { get; set; }
    public string TypeEmoji { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
}