using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Interfaces;
using PensionSystem.Domain.Exceptions;

namespace PensionSystem.Application.Members.Handlers;

/// <summary>
/// Orchestrates member statement generation, gathering inputs from
/// Member, Contribution, and Benefit repositories.
/// </summary>
public class GetMemberStatementQueryHandler : IRequestHandler<Queries.GetMemberStatementQuery, StatementDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IContributionRepository _contributionRepository;
    private readonly IBenefitRepository _benefitRepository;

    public GetMemberStatementQueryHandler(
        IMemberRepository memberRepository,
        IContributionRepository contributionRepository,
        IBenefitRepository benefitRepository)
    {
        _memberRepository = memberRepository;
        _contributionRepository = contributionRepository;
        _benefitRepository = benefitRepository;
    }

    public async Task<StatementDto> Handle(Queries.GetMemberStatementQuery request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member == null)
            throw new DomainException($"Member with ID {request.MemberId} not found.");

        var contributions = await _contributionRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);
        var benefits = await _benefitRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);

        var totalContributions = contributions.Sum(c => c.Amount);
        var totalBenefits = benefits.Sum(b => b.Amount);

        return new StatementDto
        {
            MemberId = member.Id,
            FullName = $"{member.FirstName} {member.LastName}",
            TotalContributions = totalContributions,
            TotalBenefitsWithdrawn = totalBenefits,
            CurrentBalance = totalContributions - totalBenefits,
            RecentContributions = contributions.Select(c => new ContributionDto
            {
                Id = c.Id,
                MemberId = c.MemberId,
                Type = (int)c.Type,
                Amount = c.Amount,
                ContributionDate = c.ContributionDate,
                ReferenceNumber = c.ReferenceNumber
            }).ToList(),
            BenefitsHistory = benefits.Select(b => new BenefitDto
            {
                Id = b.Id,
                MemberId = b.MemberId,
                Type = (int)b.Type,
                CalculationDate = b.CalculationDate,
                Status = (int)b.Status,
                Amount = b.Amount
            }).ToList()
        };
    }
}
