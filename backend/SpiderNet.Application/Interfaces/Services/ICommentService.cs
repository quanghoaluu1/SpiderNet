using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Domain.Common;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.Interfaces.Services;

public interface ICommentService
{
    Task<Result<CommentDto>> AddCommentAsync(Guid postId, Guid userId, CreateCommentRequest request);
    Task<Result<bool>> DeleteCommentAsync(Guid commentId, Guid userId);

    Task<Result<IEnumerable<CommentDto>>> GetPostCommentsAsync(Guid postId, Guid? currentUserId = null,
        int pageNumber = 1, int pageSize = 20);

    Task<Result<IEnumerable<CommentDto>>> GetCommentRepliesAsync(Guid parentCommentId, Guid? currentUserId = null,
        int pageNumber = 1, int pageSize = 10);
    Task<Result<CommentReactionDto>> AddCommentReactionAsync(Guid commentId, Guid userId, ReactionType reactionType);
    Task<Result<CommentDto>> UpdateCommentAsync(Guid commentId, Guid userId, UpdateCommentRequest request);
    Task<Result<bool>> RemoveCommentReactionAsync(Guid commentId, Guid userId);

    Task<Result<IEnumerable<CommentReactionDto>>> GetCommentReactionsAsync(Guid commentId, ReactionType? type = null,
        int limit = 20);
}