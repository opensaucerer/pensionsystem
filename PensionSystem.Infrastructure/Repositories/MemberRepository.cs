using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.Interfaces;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the member repository.
/// Soft-delete is respected by filtering on IsDeleted.
/// </summary>
public class MemberRepository : IMemberRepository
{
    private readonly PensionDbContext _context;

    public MemberRepository(PensionDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Members.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Member?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Members.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await _context.Members.AddAsync(member, cancellationToken);
    }

    public void Update(Member member)
    {
        _context.Members.Update(member);
    }
}
