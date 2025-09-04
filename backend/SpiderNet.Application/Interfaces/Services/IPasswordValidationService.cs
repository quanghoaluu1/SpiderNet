using SpiderNet.Application.DTOs.Auth;

namespace SpiderNet.Application.Interfaces;

public interface IPasswordValidationService
{
    ValidationResult ValidatePassword(string password);
}