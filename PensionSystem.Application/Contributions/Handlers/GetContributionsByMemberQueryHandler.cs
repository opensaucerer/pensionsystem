using MediatR;
using PensionSystem.Application.Contributions.Queries;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Interfaces;

namespace PensionSystem.Application.Contributions.Handlers;

/// <summary>
/// Handles retrieval of all contributions belonging to a given member.
/// </summary>
public class GetContributionsByMemberQueryHandler : IRequestHandler<GetContributionsByMemberQuery, List<ContributionDto>>
{
    private readonly IContributionRepository _contributionRepository;

    public GetContributionsByMemberQueryHandler(IContributionRepository contributionRepository)
    {
        _contributionRepository = contributionRepository;
    }

    public async Task<List<ContributionDto>> Handle(GetContributionsByMemberQuery request, CancellationToken cancellationToken)
    {
        var contributions = await _contributionRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);

        return contributions.Select(c => new ContributionDto
        {
            Id = c.Id,
            MemberId = c.MemberId,
            Type = (int)c.Type,
            Amount = c.Amount,
            ContributionDate = c.ContributionDate,
            ReferenceNumber = c.ReferenceNumber
        }).ToList();
    }
}
