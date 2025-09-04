using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Reaction;

public class ReactionDto
{
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    public ReactionType Type { get; set; }
    public string TypeEmoji { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}