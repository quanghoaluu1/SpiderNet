using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiderNet.Application.DTOs.Post;
using SpiderNet.Application.Interfaces.Services;
using SpiderNet.Domain.Enum;

namespace spidernet_be.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    public PostController(IPostService postService)
    {
        _postService = postService;
    }
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _postService.CreatePostAsync(userId, request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return CreatedAtAction(nameof(GetPost), new { id = result.Data.Id }, result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPost(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _postService.GetPostAsync(id, currentUserId);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _postService.UpdatePostAsync(id, userId, request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _postService.DeletePostAsync(id, userId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = "Post deleted successfully" });
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetNewsFeed([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var userId = GetCurrentUserId();
        var result = await _postService.GetNewsFeedAsync(userId, page, size);

        return Ok(result.Data);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserPosts(Guid userId, [FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _postService.GetUserPostsAsync(userId, currentUserId, page, size);

        return Ok(result.Data);
    }
    
    [HttpPost("{id}/reactions")]
    public async Task<IActionResult> AddReaction(Guid id, [FromBody] AddReactionRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _postService.AddReactionAsync(id, userId, request.Type);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { 
            reaction = result.Data,
            message = result.Data == null ? "Reaction removed" : "Reaction added"
        });
    }

    [HttpDelete("{id}/reactions")]
    public async Task<IActionResult> RemoveReaction(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _postService.RemoveReactionAsync(id, userId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = "Reaction removed successfully" });
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetPostReactions(Guid id, [FromQuery] ReactionType? type = null, [FromQuery] int limit = 20)
    {
        var result = await _postService.GetPostReactionsAsync(id, type, limit);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
    
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}