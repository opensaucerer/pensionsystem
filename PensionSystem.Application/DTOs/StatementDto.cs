namespace PensionSystem.Application.DTOs;

public class StatementDto
{
    public Guid MemberId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public decimal TotalContributions { get; set; }
    public decimal TotalBenefitsWithdrawn { get; set; }
    public decimal CurrentBalance { get; set; }
    
    public List<ContributionDto> RecentContributions { get; set; } = new();
    public List<BenefitDto> BenefitsHistory { get; set; } = new();
}
