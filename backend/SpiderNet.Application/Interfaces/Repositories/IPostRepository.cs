using SpiderNet.Domain.Entities;

namespace SpiderNet.Application.Interfaces.Repositories;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task<Post?> GetByIdAsync(Guid id);
    Task<Post?> GetDetailByIdAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Post>> GetAllAsync();
    Task<IEnumerable<Post>> GetUserPostsAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
    Task<IEnumerable<Post>> GetNewsFeedAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
    Task<bool> IsPostOwnerAsync(Guid postId, Guid userId);
}