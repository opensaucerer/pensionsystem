using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PensionSystem.Domain.Enums;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Infrastructure.Jobs;

/// <summary>
/// Hangfire recurring job: validates monthly contributions each period
/// and generates a summary report in the logs.
/// </summary>
public class MonthlyContributionValidationJob
{
    private readonly PensionDbContext _context;
    private readonly ILogger<MonthlyContributionValidationJob> _logger;

    public MonthlyContributionValidationJob(PensionDbContext context, ILogger<MonthlyContributionValidationJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Validates that all active members have submitted their mandatory monthly contribution.
    /// Runs as a Hangfire recurring job on the 1st of each month.
    /// </summary>
    public async Task ExecuteAsync()
    {
        var now = DateTime.UtcNow;
        var previousMonth = now.AddMonths(-1);
        var year = previousMonth.Year;
        var month = previousMonth.Month;

        _logger.LogInformation("Running monthly contribution validation for {Year}-{Month:D2}", year, month);

        var activeMembers = await _context.Members
            .Where(m => !m.IsDeleted)
            .ToListAsync();

        var membersMissingContribution = 0;

        foreach (var member in activeMembers)
        {
            var hasContribution = await _context.Contributions.AnyAsync(c =>
                c.MemberId == member.Id &&
                c.Type == ContributionType.Monthly &&
                c.ContributionDate.Year == year &&
                c.ContributionDate.Month == month &&
                !c.IsDeleted);

            if (!hasContribution)
            {
                membersMissingContribution++;
                _logger.LogWarning("Member {MemberId} ({Name}) is missing mandatory contribution for {Year}-{Month:D2}",
                    member.Id, $"{member.FirstName} {member.LastName}", year, month);
            }
        }

        _logger.LogInformation("Monthly validation complete. {Missing}/{Total} members missing contributions.",
            membersMissingContribution, activeMembers.Count);
    }
}
