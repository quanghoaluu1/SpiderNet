namespace SpiderNet.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public Guid UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
}