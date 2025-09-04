using Microsoft.EntityFrameworkCore;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Domain.Entities;

namespace SpiderNet.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;
    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Comment> CreateAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Reactions)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<Comment?> GetDetailByIdAsync(Guid id)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Reactions).ThenInclude(r => r.User)
            .Include(c => c.Replies.Where(r => !r.IsDeleted))
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<Comment> UpdateAsync(Comment comment)
    {
        comment.UpdatedAt = DateTime.UtcNow;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return false;

        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId, int pageNumber = 1, int pageSize = 20)
    {
        // Only get top-level comments (no parent)
        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Reactions)
            .Include(c => c.Replies.Where(r => !r.IsDeleted).Take(3)) // Load first 3 replies
            .ThenInclude(r => r.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.Reactions)
            .Where(c => c.PostId == postId && !c.IsDeleted && c.ParentCommentId == null)
            .OrderBy(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetCommentRepliesAsync(Guid parentCommentId, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Reactions)
            .Where(c => c.ParentCommentId == parentCommentId && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> IsCommentOwnerAsync(Guid commentId, Guid userId)
    {
        return await _context.Comments
            .AnyAsync(c => c.Id == commentId && c.UserId == userId && !c.IsDeleted);
    }

    public async Task<int> GetRepliesCountAsync(Guid commentId)
    {
        return await _context.Comments
            .CountAsync(c => c.ParentCommentId == commentId && !c.IsDeleted);
    }
}