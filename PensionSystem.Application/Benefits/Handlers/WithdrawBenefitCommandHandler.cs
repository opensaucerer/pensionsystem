using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Interfaces;
using PensionSystem.Domain.Entities;
using PensionSystem.Domain.Enums;
using PensionSystem.Domain.Exceptions;

namespace PensionSystem.Application.Benefits.Handlers;

/// <summary>
/// Handles a member's request to withdraw benefits. Enforces rules regarding 
/// sufficient current balance and a minimum of 12 monthly contributions.
/// </summary>
public class WithdrawBenefitCommandHandler : IRequestHandler<Commands.WithdrawBenefitCommand, BenefitDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IContributionRepository _contributionRepository;
    private readonly IBenefitRepository _benefitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawBenefitCommandHandler(
        IMemberRepository memberRepository,
        IContributionRepository contributionRepository,
        IBenefitRepository benefitRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _contributionRepository = contributionRepository;
        _benefitRepository = benefitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BenefitDto> Handle(Commands.WithdrawBenefitCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member == null)
            throw new DomainException($"Member with ID {request.MemberId} not found.");

        if (request.Amount <= 0)
            throw new DomainException("Withdrawal amount must be greater than zero.");

        var contributions = await _contributionRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);
        var benefits = await _benefitRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);

        var totalContributions = contributions.Sum(c => c.Amount);
        var totalBenefitsWithdrawn = benefits.Sum(b => b.Amount);
        var currentBalance = totalContributions - totalBenefitsWithdrawn;

        if (request.Amount > currentBalance)
            throw new DomainException($"Insufficient balance for withdrawal. Current available balance is {currentBalance:C}.");
            
        // Check 12-month rule
        var monthlyCount = contributions.Count(c => c.Type == ContributionType.Monthly);
        if (monthlyCount < 12)
            throw new DomainException("Member must have at least 12 monthly contributions to be eligible to withdraw benefits.");

        var benefit = new Benefit(
            request.MemberId,
            request.Type,
            DateTime.UtcNow,
            EligibilityStatus.Eligible,
            request.Amount
        );

        await _benefitRepository.AddAsync(benefit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BenefitDto
        {
            Id = benefit.Id,
            MemberId = benefit.MemberId,
            Type = (int)benefit.Type,
            CalculationDate = benefit.CalculationDate,
            Status = (int)benefit.Status,
            Amount = benefit.Amount
        };
    }
}
