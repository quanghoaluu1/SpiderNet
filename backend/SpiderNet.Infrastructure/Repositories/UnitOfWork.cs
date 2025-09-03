using SpiderNet.Application.Interfaces;

namespace SpiderNet.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IUserRepository UserRepository { get; }
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        UserRepository = new UserRepository(_context);
    }
    
}