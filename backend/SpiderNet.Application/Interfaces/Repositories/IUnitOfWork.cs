using SpiderNet.Application.Interfaces.Repositories;

namespace SpiderNet.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IPostRepository PostRepository { get; }
    ICommentRepository CommentRepository { get; }
    IReactionRepository ReactionRepository { get; }
}