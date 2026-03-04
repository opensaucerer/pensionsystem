namespace PensionSystem.Application.DTOs;

public class ContributionDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public int Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
}
