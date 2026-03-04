using PensionSystem.Domain.Common;
using PensionSystem.Domain.Enums;

namespace PensionSystem.Domain.Entities;

public class Benefit : BaseEntity
{
    public Guid MemberId { get; private set; }
    public BenefitType Type { get; private set; }
    public DateTime CalculationDate { get; private set; }
    public EligibilityStatus Status { get; private set; }
    public decimal Amount { get; private set; }

    public Member Member { get; private set; } = null!;

    private Benefit() { }

    public Benefit(Guid memberId, BenefitType type, DateTime calculationDate, EligibilityStatus status, decimal amount)
    {
        Id = Guid.NewGuid();
        MemberId = memberId;
        Type = type;
        CalculationDate = calculationDate;
        Status = status;
        Amount = amount;
    }

    public void UpdateStatus(EligibilityStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
