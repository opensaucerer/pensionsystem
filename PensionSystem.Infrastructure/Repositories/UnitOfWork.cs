using PensionSystem.Application.Interfaces;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PensionDbContext _context;

    public UnitOfWork(PensionDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
