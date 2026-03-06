using MediatR;
using PensionSystem.Application.Interfaces;
using PensionSystem.Application.Members.Commands;
using PensionSystem.Domain.Exceptions;

namespace PensionSystem.Application.Members.Handlers;

/// <summary>
/// Handles soft-deleting a member by setting the IsDeleted flag.
/// </summary>
public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, bool>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMemberCommandHandler(IMemberRepository memberRepository, IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException($"Member with ID {request.Id} not found.");

        member.IsDeleted = true;
        member.DeletedAt = DateTime.UtcNow;
        _memberRepository.Update(member);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
