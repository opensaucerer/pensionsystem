using PensionSystem.Domain.Entities;

namespace PensionSystem.Application.Interfaces;

public interface IContributionRepository
{
    Task<Contribution?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Contribution>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<bool> HasMonthlyContributionInMonthAsync(Guid memberId, int year, int month, CancellationToken cancellationToken = default);
    Task AddAsync(Contribution contribution, CancellationToken cancellationToken = default);
}
