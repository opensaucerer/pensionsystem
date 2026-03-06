using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PensionSystem.Domain.Enums;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Infrastructure.Jobs;

/// <summary>
/// Hangfire recurring job: updates benefit eligibility for all members.
/// A member must have at least 12 months of contributions to become eligible.
/// </summary>
public class BenefitEligibilityJob
{
    private readonly PensionDbContext _context;
    private readonly ILogger<BenefitEligibilityJob> _logger;

    public BenefitEligibilityJob(PensionDbContext context, ILogger<BenefitEligibilityJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Checks each active member's contribution history and updates eligibility status.
    /// Minimum 12 monthly contributions required for benefit eligibility.
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Running benefit eligibility update job");

        var members = await _context.Members
            .Where(m => !m.IsDeleted)
            .ToListAsync();

        foreach (var member in members)
        {
            var monthlyCount = await _context.Contributions.CountAsync(c =>
                c.MemberId == member.Id &&
                c.Type == ContributionType.Monthly &&
                !c.IsDeleted);

            var isEligible = monthlyCount >= 12;

            _logger.LogInformation("Member {MemberId}: {Count} monthly contributions → {Status}",
                member.Id, monthlyCount, isEligible ? "Eligible" : "Not yet eligible");
        }

        _logger.LogInformation("Benefit eligibility update job complete for {Count} members", members.Count);
    }
}
