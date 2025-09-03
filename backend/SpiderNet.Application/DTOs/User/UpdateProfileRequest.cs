namespace SpiderNet.Application.DTOs.User;

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
}