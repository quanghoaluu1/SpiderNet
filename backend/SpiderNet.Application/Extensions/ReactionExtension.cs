using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.Extensions;

public static class ReactionExtension
{
    public static string GetEmoji(this ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "👍",
            ReactionType.Love => "❤️",
            ReactionType.Haha => "😂",
            ReactionType.Wow => "😮",
            ReactionType.Sad => "😢",
            ReactionType.Angry => "😠",
            _ => "👍"
        };
    }
    
    public static string GetDisplayName(this ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "Like",
            ReactionType.Love => "Love",
            ReactionType.Haha => "Haha",
            ReactionType.Wow => "Wow",
            ReactionType.Sad => "Sad",
            ReactionType.Angry => "Angry",
            _ => "Like"
        };
    }
    
    public static string GetColor(this ReactionType reactionType)
    {
        return reactionType switch
        {
            ReactionType.Like => "#1877F2", // Facebook blue
            ReactionType.Love => "#E41E3F", // Red
            ReactionType.Haha => "#FDD835", // Yellow
            ReactionType.Wow => "#FDD835", // Yellow
            ReactionType.Sad => "#FDD835", // Yellow
            ReactionType.Angry => "#E9710F", // Orange
            _ => "#1877F2"
        };
    }
}