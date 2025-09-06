using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Application.Interfaces.Services;
using SpiderNet.Domain.Enum;

namespace spidernet_be.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("posts/{postId}")]
    public async Task<IActionResult> GetPostComments(Guid postId, [FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _commentService.GetPostCommentsAsync(postId, currentUserId, page, size);

        return Ok(result.Data);
    }

    [HttpGet("{commentId}/replies")]
    public async Task<IActionResult> GetCommentReplies(Guid commentId, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _commentService.GetCommentRepliesAsync(commentId, currentUserId, page, size);

        return Ok(result.Data);
    }

    [HttpPost("posts/{postId}")]
    public async Task<IActionResult> AddComment(Guid postId, [FromForm] CreateCommentRequest request)
    {
        // Validate media files
        if (request.Image != null)
        {
            var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedImageTypes.Contains(request.Image.ContentType.ToLower()))
                return BadRequest(new { message = "Only JPEG, PNG, GIF, and WebP images are allowed" });
        
            if (request.Image.Length > 10 * 1024 * 1024) // 10MB
                return BadRequest(new { message = "Image size cannot exceed 10MB" });
        }

        if (request.Video != null)
        {
            var allowedVideoTypes = new[] { "video/mp4", "video/webm", "video/ogg", "video/avi", "video/mov" };
            if (!allowedVideoTypes.Contains(request.Video.ContentType.ToLower()))
                return BadRequest(new { message = "Only MP4, WebM, OGG, AVI, and MOV videos are allowed" });
        
            if (request.Video.Length > 100 * 1024 * 1024) // 100MB
                return BadRequest(new { message = "Video size cannot exceed 100MB" });
        }

        if (request.Gif != null)
        {
            if (request.Gif.ContentType.ToLower() != "image/gif")
                return BadRequest(new { message = "Only GIF files are allowed" });
        
            if (request.Gif.Length > 20 * 1024 * 1024) // 20MB
                return BadRequest(new { message = "GIF size cannot exceed 20MB" });
        }

        var userId = GetCurrentUserId();
        var result = await _commentService.AddCommentAsync(postId, userId, request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _commentService.UpdateCommentAsync(commentId, userId, request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = GetCurrentUserId();
        var result = await _commentService.DeleteCommentAsync(commentId, userId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = "Comment deleted successfully" });
    }

    // Comment reactions
    [HttpPost("{commentId}/reactions")]
    public async Task<IActionResult> AddCommentReaction(Guid commentId, [FromBody] AddCommentReactionRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _commentService.AddCommentReactionAsync(commentId, userId, request.Type);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { 
            reaction = result.Data,
            message = result.Data == null ? "Reaction removed" : "Reaction added"
        });
    }

    [HttpDelete("{commentId}/reactions")]
    public async Task<IActionResult> RemoveCommentReaction(Guid commentId)
    {
        var userId = GetCurrentUserId();
        var result = await _commentService.RemoveCommentReactionAsync(commentId, userId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = "Reaction removed successfully" });
    }

    [HttpGet("{commentId}/reactions")]
    public async Task<IActionResult> GetCommentReactions(Guid commentId, [FromQuery] ReactionType? type = null, [FromQuery] int limit = 20)
    {
        var result = await _commentService.GetCommentReactionsAsync(commentId, type, limit);

        return Ok(result.Data);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}