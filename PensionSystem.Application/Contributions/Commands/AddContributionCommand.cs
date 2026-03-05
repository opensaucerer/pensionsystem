using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Domain.Enums;

namespace PensionSystem.Application.Contributions.Commands;

public record AddContributionCommand(
    Guid MemberId,
    ContributionType Type,
    decimal Amount,
    DateTime ContributionDate,
    string ReferenceNumber) : IRequest<ContributionDto>;
