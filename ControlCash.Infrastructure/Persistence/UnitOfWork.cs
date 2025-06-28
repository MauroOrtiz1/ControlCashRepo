using ControlCash.Domain.Interfaces.UnitOfWork;

namespace ControlCash.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ControlCashDbContext _context;

    public UnitOfWork(ControlCashDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}