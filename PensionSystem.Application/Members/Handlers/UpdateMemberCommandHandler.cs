using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Interfaces;
using PensionSystem.Application.Members.Commands;
using PensionSystem.Domain.Exceptions;

namespace PensionSystem.Application.Members.Handlers;

/// <summary>
/// Handles member update by applying new details to an existing member entity.
/// </summary>
public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMemberCommandHandler(IMemberRepository memberRepository, IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException($"Member with ID {request.Id} not found.");

        member.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Email,
            request.PhoneNumber);

        _memberRepository.Update(member);
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
