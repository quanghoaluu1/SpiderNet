namespace SpiderNet.Domain.Entities;


public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly? Birthday { get; set; }
    public string Gender { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; } = false;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    
    public bool IsPrivate { get; set; } = false; 
    public bool ShowEmail { get; set; } = false;
    public bool ShowPhoneNumber { get; set; } = false;
    public bool ShowDateOfBirth { get; set; } = false;
    
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}