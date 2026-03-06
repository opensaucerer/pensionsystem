using MediatR;
using PensionSystem.Application.DTOs;

namespace PensionSystem.Application.Contributions.Queries;

/// <summary>
/// Query to retrieve all contributions for a specific member.
/// </summary>
public record GetContributionsByMemberQuery(Guid MemberId) : IRequest<List<ContributionDto>>;
