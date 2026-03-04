namespace PensionSystem.Application.DTOs;

public class BenefitDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public int Type { get; set; }
    public DateTime CalculationDate { get; set; }
    public int Status { get; set; }
    public decimal Amount { get; set; }
}
