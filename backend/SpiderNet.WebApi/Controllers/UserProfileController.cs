using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SpiderNet.Application.DTOs.User;
using SpiderNet.Application.Interfaces;

namespace spidernet_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }
    [HttpGet("profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetCurrentUserId();
        var result = await _userProfileService.GetUserProfileAsync(userId, userId);
        
        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserProfile(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _userProfileService.GetUserProfileAsync(id, currentUserId);
        
        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _userProfileService.UpdateProfileAsync(userId, request);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _userProfileService.ChangePasswordAsync(userId, request);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(new { message = "Password changed successfully" });
    }
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}