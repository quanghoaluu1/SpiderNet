using System.Text.RegularExpressions;
using SpiderNet.Application.DTOs.Auth;
using SpiderNet.Application.Interfaces;

namespace SpiderNet.Application.Services;

public partial class PasswordValidationService : IPasswordValidationService
{
    private readonly int _minLength = 8;
    private readonly int _maxLength = 64;

    public ValidationResult ValidatePassword(string password)
    {
        try
        {
            var result = new ValidationResult { IsValid = true };
            
            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password cannot be empty.");
                return result;
            }
            
            if (password.Length < _minLength)
            {
                result.IsValid = false;
                result.Errors.Add($"Password must be at least {_minLength} characters long.");
            }

            if (password.Length > _maxLength)
            {
                result.IsValid = false;
                result.Errors.Add($"Password cannot exceed {_maxLength} characters.");
            }
            
            if (!UpperCaseRegex().IsMatch(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one uppercase letter.");
            }

            if (!LowerCaseRegex().IsMatch(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one lowercase letter.");
            }
            
            if (!OneNumberRegex().IsMatch(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one number");
            }
            
            if (!SpecialCharacterRegex().IsMatch(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one special character");
            }
            
            var commonPasswords = new HashSet<string>
            {
                "password", "123456", "123456789", "qwerty", "abc123", "football",
                "12345678", "111111", "1234567", "iloveyou", "adobe123", "123123",
                "admin", "welcome", "login", "letmein", "monkey", "dragon"
            };
            
            if (commonPasswords.Any(cp => password.ToLower().Contains(cp)))
            {
                result.IsValid = false;
                result.Errors.Add("Password is too common. Please choose a more secure password.");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            var errorResult = new ValidationResult { IsValid = false };
            errorResult.Errors.Add($"An error occurred during password validation: {ex.Message}");
            return errorResult;
        }
    }

    [GeneratedRegex(@"[a-z]")]
    private static partial Regex LowerCaseRegex();
    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UpperCaseRegex();
    [GeneratedRegex(@"\d")]
    private static partial Regex OneNumberRegex();
    [GeneratedRegex(@"[!@#$%^&*(),.?""{}|<>]")]
    private static partial Regex SpecialCharacterRegex();
}