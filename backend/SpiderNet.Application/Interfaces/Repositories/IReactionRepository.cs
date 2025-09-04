using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Domain.Entities;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.Interfaces.Repositories;

public interface IReactionRepository
{
    Task<Reaction?> GetReactionAsync(Guid postId, Guid userId);
    Task<Reaction> AddReactionAsync(Reaction reaction);
    Task<Reaction> UpdateReactionAsync(Reaction reaction);
    Task<bool> RemoveReactionAsync(Guid postId, Guid userId);
    Task<IEnumerable<Reaction>> GetPostReactionsAsync(Guid postId, int limit = 20);
    Task<ReactionSummaryDto> GetReactionsSummaryAsync(Guid postId);
    Task<IEnumerable<Reaction>> GetReactionsByTypeAsync(Guid postId, ReactionType type, int limit = 20);
}