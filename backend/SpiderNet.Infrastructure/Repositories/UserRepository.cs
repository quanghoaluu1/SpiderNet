using Microsoft.EntityFrameworkCore;
using SpiderNet.Application.Interfaces;
using SpiderNet.Domain.Entities;

namespace SpiderNet.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id && u.IsActive); 
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return null;
        }
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        if (string.IsNullOrEmpty(usernameOrEmail))
        {
            return null;
        }
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => (u.Username.ToLower() == usernameOrEmail.ToLower() || u.Email.ToLower() == usernameOrEmail.ToLower()) && u.IsActive);
    }

    public async Task<User> CreateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.IsActive = true;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistAsync(string email, string username)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(username))
            return false;

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => 
                (u.Email.ToLower() == email.ToLower() || 
                 u.Username.ToLower() == username.ToLower()) 
                && u.IsActive);
    }

    public async Task<bool> ExistByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }

    public async Task<bool> ExistByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public async Task<User?> GetUserProfileAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<User> UpdateProfileAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}