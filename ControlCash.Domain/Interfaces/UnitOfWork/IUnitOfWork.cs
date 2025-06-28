namespace ControlCash.Domain.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    Task CommitAsync();
}