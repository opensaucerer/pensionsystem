using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Interfaces;
using PensionSystem.Domain.Entities;
using PensionSystem.Domain.Enums;
using PensionSystem.Domain.Exceptions;

namespace PensionSystem.Application.Contributions.Handlers;

public class AddContributionCommandHandler : IRequestHandler<Commands.AddContributionCommand, ContributionDto>
{
    private readonly IContributionRepository _contributionRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddContributionCommandHandler(
        IContributionRepository contributionRepository,
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _contributionRepository = contributionRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ContributionDto> Handle(Commands.AddContributionCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member == null)
            throw new DomainException($"Member with ID {request.MemberId} not found.");

        if (request.Type == ContributionType.Monthly)
        {
            var hasMonthly = await _contributionRepository.HasMonthlyContributionInMonthAsync(
                request.MemberId, 
                request.ContributionDate.Year, 
                request.ContributionDate.Month, 
                cancellationToken);

            if (hasMonthly)
                throw new DomainException("A monthly contribution has already been made for this month.");
        }

        var contribution = new Contribution(
            request.MemberId,
            request.Type,
            request.Amount,
            request.ContributionDate,
            request.ReferenceNumber);

        await _contributionRepository.AddAsync(contribution, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ContributionDto
        {
            Id = contribution.Id,
            MemberId = contribution.MemberId,
            Type = (int)contribution.Type,
            Amount = contribution.Amount,
            ContributionDate = contribution.ContributionDate,
            ReferenceNumber = contribution.ReferenceNumber
        };
    }
}
