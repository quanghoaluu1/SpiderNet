namespace SpiderNet.Application.DTOs.User;

public class UpdatePrivacySettingsRequest
{
    public bool IsPrivate { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowPhoneNumber { get; set; }
    public bool ShowDateOfBirth { get; set; }
}