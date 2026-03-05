using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Interfaces;

namespace PensionSystem.Application.Members.Handlers;

public class GetMemberByIdQueryHandler : IRequestHandler<Queries.GetMemberByIdQuery, MemberDto?>
{
    private readonly IMemberRepository _memberRepository;

    public GetMemberByIdQueryHandler(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<MemberDto?> Handle(Queries.GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (member == null) return null;

        return new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            DateOfBirth = member.DateOfBirth,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            Age = member.GetAge()
        };
    }
}
