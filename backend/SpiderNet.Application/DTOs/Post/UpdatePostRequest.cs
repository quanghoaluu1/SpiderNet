using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Post;

public class UpdatePostRequest
{
    public string Content { get; set; } = string.Empty;
    public PostPrivacy Privacy { get; set; } = PostPrivacy.Public;
}