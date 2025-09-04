using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment> CreateAsync(Comment comment);
    Task<Comment?> GetByIdAsync(Guid id);
    Task<Comment?> GetDetailByIdAsync(Guid id);
    Task<Comment> UpdateAsync(Comment comment);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Comment>> GetPostCommentsAsync(Guid postId, int pageNumber = 1, int pageSize = 20);
    Task<IEnumerable<Comment>> GetCommentRepliesAsync(Guid parentCommentId, int pageNumber = 1, int pageSize = 10);
    Task<bool> IsCommentOwnerAsync(Guid commentId, Guid userId);
    Task<int> GetRepliesCountAsync(Guid commentId);
}