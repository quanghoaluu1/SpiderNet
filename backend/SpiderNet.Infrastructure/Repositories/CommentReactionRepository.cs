using Microsoft.EntityFrameworkCore;
using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Application.Extensions;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Domain.Entities;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Infrastructure.Repositories;

public class CommentReactionRepository : ICommentReactionRepository
{
    private readonly AppDbContext _context;
    public CommentReactionRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<CommentReaction?> GetReactionAsync(Guid commentId, Guid userId)
    {
        return await _context.Set<CommentReaction>()
            .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId);
    }

    public async Task<CommentReaction> AddReactionAsync(CommentReaction reaction)
    {
        _context.Set<CommentReaction>().Add(reaction);
        await _context.SaveChangesAsync();
        return reaction;
    }

    public async Task<CommentReaction> UpdateReactionAsync(CommentReaction reaction)
    {
        reaction.UpdatedAt = DateTime.UtcNow;
        _context.Set<CommentReaction>().Update(reaction);
        await _context.SaveChangesAsync();
        return reaction;
    }

    public async Task<bool> RemoveReactionAsync(Guid commentId, Guid userId)
    {
        var reaction = await GetReactionAsync(commentId, userId);
        if (reaction == null) return false;

        _context.Set<CommentReaction>().Remove(reaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CommentReaction>> GetCommentReactionsAsync(Guid commentId, int limit = 20)
    {
        return await _context.Set<CommentReaction>()
            .Include(r => r.User)
            .Where(r => r.CommentId == commentId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<CommentReactionSummaryDto> GetReactionsSummaryAsync(Guid commentId)
    {
        var reactions = await _context.Set<CommentReaction>()
            .Where(r => r.CommentId == commentId)
            .GroupBy(r => r.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        var summary = new CommentReactionSummaryDto
        {
            TotalCount = reactions.Sum(r => r.Count),
            LikesCount = reactions.FirstOrDefault(r => r.Type == ReactionType.Like)?.Count ?? 0,
            LovesCount = reactions.FirstOrDefault(r => r.Type == ReactionType.Love)?.Count ?? 0,
            HahaCount = reactions.FirstOrDefault(r => r.Type == ReactionType.Haha)?.Count ?? 0,
            WowCount = reactions.FirstOrDefault(r => r.Type == ReactionType.Wow)?.Count ?? 0,
            SadCount = reactions.FirstOrDefault(r => r.Type == ReactionType.Sad)?.Count ?? 0,
            AngryCount = reactions.FirstOrDefault(r => r.Type == ReactionType.Angry)?.Count ?? 0
        };

        // Top 3 reactions for display
        summary.TopReactions = reactions
            .OrderByDescending(r => r.Count)
            .Take(3)
            .Select(r => new ReactionTypeCount
            {
                Type = r.Type,
                TypeEmoji = r.Type.GetEmoji(),
                TypeName = r.Type.GetDisplayName(),
                Count = r.Count
            })
            .ToList();

        return summary;
    }

    public async Task<IEnumerable<CommentReaction>> GetReactionsByTypeAsync(Guid commentId, ReactionType type, int limit = 20)
    {
        return await _context.Set<CommentReaction>()
            .Include(r => r.User)
            .Where(r => r.CommentId == commentId && r.Type == type)
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
}