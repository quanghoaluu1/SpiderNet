namespace SpiderNet.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
}