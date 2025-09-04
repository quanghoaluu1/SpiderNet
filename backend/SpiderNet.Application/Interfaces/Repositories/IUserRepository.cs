using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistAsync(string email, string username);
    Task<bool> ExistByEmailAsync(string email);
    Task<bool> ExistByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize);
    Task<User?> GetUserProfileAsync(Guid id);
    Task<User> UpdateProfileAsync(User user);
}