using PensionSystem.Domain.Entities;

namespace PensionSystem.Application.Interfaces;

public interface IBenefitRepository
{
    Task<List<Benefit>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task AddAsync(Benefit benefit, CancellationToken cancellationToken = default);
}
