using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Infrastructure.Jobs;

/// <summary>
/// Hangfire recurring job: applies simple monthly interest (0.5%) to all member contributions.
/// </summary>
public class MonthlyInterestCalculationJob
{
    private readonly PensionDbContext _context;
    private readonly ILogger<MonthlyInterestCalculationJob> _logger;

    private const decimal MonthlyInterestRate = 0.005m; // 0.5% per month

    public MonthlyInterestCalculationJob(PensionDbContext context, ILogger<MonthlyInterestCalculationJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Calculates total contributions per member and logs the interest earned.
    /// In a real system, this would create interest credit entries.
    /// </summary>
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Running monthly interest calculation job");

        var members = await _context.Members
            .Where(m => !m.IsDeleted)
            .ToListAsync();

        foreach (var member in members)
        {
            var totalContributions = await _context.Contributions
                .Where(c => c.MemberId == member.Id && !c.IsDeleted)
                .SumAsync(c => c.Amount);

            var interest = totalContributions * MonthlyInterestRate;

            if (totalContributions > 0)
            {
                _logger.LogInformation("Member {MemberId}: Total contributions={Total:C}, Interest earned={Interest:C}",
                    member.Id, totalContributions, interest);
            }
        }

        _logger.LogInformation("Monthly interest calculation complete for {Count} members", members.Count);
    }
}
