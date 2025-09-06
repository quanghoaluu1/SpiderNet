using Microsoft.AspNetCore.Http;

namespace SpiderNet.Application.DTOs.Comment;

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public IFormFile? Image { get; set; }
    public IFormFile? Video { get; set; }
    public IFormFile? Gif { get; set; }
}