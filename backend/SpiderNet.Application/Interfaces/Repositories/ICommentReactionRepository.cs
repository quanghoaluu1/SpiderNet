using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Domain.Entities;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.Interfaces.Repositories;

public interface ICommentReactionRepository
{
    Task<CommentReaction?> GetReactionAsync(Guid commentId, Guid userId);
    Task<CommentReaction> AddReactionAsync(CommentReaction reaction);
    Task<CommentReaction> UpdateReactionAsync(CommentReaction reaction);
    Task<bool> RemoveReactionAsync(Guid commentId, Guid userId);
    Task<IEnumerable<CommentReaction>> GetCommentReactionsAsync(Guid commentId, int limit = 20);
    Task<CommentReactionSummaryDto> GetReactionsSummaryAsync(Guid commentId);
    Task<IEnumerable<CommentReaction>> GetReactionsByTypeAsync(Guid commentId, ReactionType type, int limit = 20);
}