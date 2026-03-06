using MediatR;

namespace PensionSystem.Application.Members.Commands;

/// <summary>
/// Command to soft-delete a member.
/// </summary>
public record DeleteMemberCommand(Guid Id) : IRequest<bool>;
