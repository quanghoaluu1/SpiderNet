namespace SpiderNet.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IPostRepository PostRepository { get; }
    ICommentRepository CommentRepository { get; }
    IReactionRepository ReactionRepository { get; }
    ICommentReactionRepository CommentReactionRepository { get; }
}