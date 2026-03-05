using MediatR;
using PensionSystem.Application.DTOs;

namespace PensionSystem.Application.Members.Queries;

public record GetMemberByIdQuery(Guid Id) : IRequest<MemberDto?>;
