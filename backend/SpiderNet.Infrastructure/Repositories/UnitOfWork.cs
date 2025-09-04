using SpiderNet.Application.Interfaces;
using SpiderNet.Application.Interfaces.Repositories;

namespace SpiderNet.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IUserRepository UserRepository { get; }
    public IPostRepository PostRepository { get; }
    public ICommentRepository CommentRepository { get; }
    public IReactionRepository ReactionRepository { get; }
    public ICommentReactionRepository CommentReactionRepository { get; }
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        UserRepository = new UserRepository(_context);
        PostRepository = new PostRepository(_context);
        CommentRepository = new CommentRepository(_context);
        ReactionRepository = new ReactionRepository(_context);
        CommentReactionRepository = new CommentReactionRepository(_context);
    }
    
}