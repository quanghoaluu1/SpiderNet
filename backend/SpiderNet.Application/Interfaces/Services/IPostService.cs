using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Application.DTOs.Post;
using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Domain.Common;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.Interfaces.Services;

public interface IPostService
{
    Task<Result<PostDto>> CreatePostAsync(Guid userId, CreatePostRequest request);
    Task<Result<PostDetailDto>> GetPostAsync(Guid postId, Guid? currentUserId = null);
    Task<Result<PostDto>> UpdatePostAsync(Guid postId, Guid userId, UpdatePostRequest request);
    Task<Result<bool>> DeletePostAsync(Guid postId, Guid userId);
    Task<Result<IEnumerable<PostDto>>> GetUserPostsAsync(Guid userId, Guid? currentUserId = null, int pageNumber = 1, int pageSize = 20);
    Task<Result<IEnumerable<PostDto>>> GetNewsFeedAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
    Task<Result<ReactionDto>> AddReactionAsync(Guid postId, Guid userId, ReactionType reactionType);
    Task<Result<bool>> RemoveReactionAsync(Guid postId, Guid userId);

    Task<Result<IEnumerable<ReactionDto>>> GetPostReactionsAsync(Guid postId, ReactionType? type = null, int limit = 20);
}