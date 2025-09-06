using Microsoft.EntityFrameworkCore;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Domain.Entities;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Post> CreateAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post> UpdateAsync(Post post)
    {
        post.UpdatedAt = DateTime.UtcNow;
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Reactions).ThenInclude(l => l.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<Post?> GetDetailByIdAsync(Guid id)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Reactions).ThenInclude(l => l.User)
            .Include(p => p.Comments.Where(c => !c.IsDeleted))
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return false;

        post.IsDeleted = true;
        post.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<IEnumerable<Post>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Post>> GetUserPostsAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetNewsFeedAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Reactions)
            .Include(p => p.Comments)
            .Where(p => !p.IsDeleted && p.Privacy == PostPrivacy.Public)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> IsPostOwnerAsync(Guid postId, Guid userId)
    {
        return await _context.Posts
            .AnyAsync(p => p.Id == postId && p.UserId == userId && !p.IsDeleted);
    }
}