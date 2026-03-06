using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Domain.Enums;

namespace PensionSystem.Application.Benefits.Commands;

/// <summary>
/// Command to request a benefit withdrawal for a member.
/// </summary>
public record WithdrawBenefitCommand(Guid MemberId, decimal Amount, BenefitType Type) : IRequest<BenefitDto>;
