using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.Interfaces;
using PensionSystem.Domain.Entities;
using PensionSystem.Domain.Enums;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the contribution repository.
/// </summary>
public class ContributionRepository : IContributionRepository
{
    private readonly PensionDbContext _context;

    public ContributionRepository(PensionDbContext context)
    {
        _context = context;
    }

    public async Task<Contribution?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Contributions.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<List<Contribution>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Contributions
            .Where(x => x.MemberId == memberId && !x.IsDeleted)
            .OrderByDescending(x => x.ContributionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasMonthlyContributionInMonthAsync(Guid memberId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _context.Contributions.AnyAsync(x =>
            x.MemberId == memberId &&
            x.Type == ContributionType.Monthly &&
            x.ContributionDate.Year == year &&
            x.ContributionDate.Month == month &&
            !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Contribution contribution, CancellationToken cancellationToken = default)
    {
        await _context.Contributions.AddAsync(contribution, cancellationToken);
    }
}
