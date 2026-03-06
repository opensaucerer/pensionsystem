using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.Interfaces;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Infrastructure.Repositories;

public class BenefitRepository : IBenefitRepository
{
    private readonly PensionDbContext _context;

    public BenefitRepository(PensionDbContext context)
    {
        _context = context;
    }

    public async Task<List<Benefit>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Benefits
            .Where(x => x.MemberId == memberId && !x.IsDeleted)
            .OrderByDescending(x => x.CalculationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Benefit benefit, CancellationToken cancellationToken = default)
    {
        await _context.Benefits.AddAsync(benefit, cancellationToken);
    }
}
