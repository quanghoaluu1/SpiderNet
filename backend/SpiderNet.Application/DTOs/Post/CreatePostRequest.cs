using Microsoft.AspNetCore.Http;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.DTOs.Post;

public class CreatePostRequest
{
    public string Content { get; set; } = string.Empty;
    public PostPrivacy Privacy { get; set; } = PostPrivacy.Public;
    public IFormFile? Image { get; set; }
    public IFormFile? Video { get; set; }
}