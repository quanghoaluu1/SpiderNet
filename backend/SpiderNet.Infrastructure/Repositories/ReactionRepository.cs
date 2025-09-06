using Microsoft.EntityFrameworkCore;
using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Domain.Entities;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Infrastructure.Repositories;

public class ReactionRepository : IReactionRepository
{
    private readonly AppDbContext _context;
    public ReactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reaction?> GetReactionAsync(Guid postId, Guid userId)
    {
        return await _context.Reactions.Include(r => r.User).Include(r => r.Post)
            .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
    }

    public async Task<Reaction> AddReactionAsync(Reaction reaction)
    {
        _context.Reactions.Add(reaction);
        await _context.SaveChangesAsync();
        return reaction;
    }

    public async Task<Reaction> UpdateReactionAsync(Reaction reaction)
    {
        reaction.UpdatedAt = DateTime.UtcNow;
        _context.Reactions.Update(reaction);
        await _context.SaveChangesAsync();
        return reaction;
    }

    public async Task<bool> RemoveReactionAsync(Guid postId, Guid userId)
    {
        var reaction = await GetReactionAsync(postId, userId);
        if (reaction == null) return false;

        _context.Reactions.Remove(reaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Reaction>> GetPostReactionsAsync(Guid postId, int limit = 20)
    {
        return await _context.Reactions
            .Include(r => r.User)
            .Where(r => r.PostId == postId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<ReactionSummaryDto> GetReactionsSummaryAsync(Guid postId)
    {
        var reactions = await _context.Reactions.Include(r => r.User)
            .Where(r => r.PostId == postId)
            .GroupBy(r => r.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        var summary = new ReactionSummaryDto
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
                TypeEmoji = GetReactionEmoji(r.Type),
                TypeName = r.Type.ToString(),
                Count = r.Count
            })
            .ToList();

        return summary;
    }

    public async Task<IEnumerable<Reaction>> GetReactionsByTypeAsync(Guid postId, ReactionType type, int limit = 20)
    {
        return await _context.Reactions
            .Include(r => r.User)
            .Where(r => r.PostId == postId && r.Type == type)
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
    private string GetReactionEmoji(ReactionType type)
    {
        return type switch
        {
            ReactionType.Like => "👍",
            ReactionType.Love => "❤️",
            ReactionType.Haha => "😂",
            ReactionType.Wow => "😮",
            ReactionType.Sad => "😢",
            ReactionType.Angry => "😠",
            _ => "👍"
        };
    }
}