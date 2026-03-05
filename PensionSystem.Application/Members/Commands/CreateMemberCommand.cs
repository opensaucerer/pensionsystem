using MediatR;
using PensionSystem.Application.DTOs;

namespace PensionSystem.Application.Members.Commands;

public record CreateMemberCommand(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Email,
    string PhoneNumber) : IRequest<MemberDto>;
