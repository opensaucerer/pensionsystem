using MediatR;
using PensionSystem.Application.DTOs;

namespace PensionSystem.Application.Members.Commands;

/// <summary>
/// Command to update an existing member's details.
/// </summary>
public record UpdateMemberCommand(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Email,
    string PhoneNumber) : IRequest<MemberDto>;
