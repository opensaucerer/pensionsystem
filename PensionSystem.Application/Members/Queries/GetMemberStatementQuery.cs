using MediatR;
using PensionSystem.Application.DTOs;

namespace PensionSystem.Application.Members.Queries;

/// <summary>
/// Retrieves a comprehensive financial statement for a member,
/// including their total balance and recent transactions.
/// </summary>
public record GetMemberStatementQuery(Guid MemberId) : IRequest<StatementDto>;
