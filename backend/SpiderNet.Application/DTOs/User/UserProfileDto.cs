namespace SpiderNet.Application.DTOs.User;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    
    // Computed fields
    public string MemberSince { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsOwnProfile { get; set; } // True if viewing own profile
}