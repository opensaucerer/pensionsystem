using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Interfaces;
using PensionSystem.Domain.Entities;
using PensionSystem.Domain.Exceptions;

namespace PensionSystem.Application.Members.Handlers;

/// <summary>
/// Creates a new pension member after verifying email uniqueness.
/// </summary>
public class CreateMemberCommandHandler : IRequestHandler<Commands.CreateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMemberCommandHandler(IMemberRepository memberRepository, IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> Handle(Commands.CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var existingMember = await _memberRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingMember != null)
            throw new DomainException("A member with this email already exists.");

        var member = new Member(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Email,
            request.PhoneNumber);

        await _memberRepository.AddAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
