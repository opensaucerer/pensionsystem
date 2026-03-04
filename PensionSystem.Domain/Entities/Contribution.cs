using PensionSystem.Domain.Common;
using PensionSystem.Domain.Enums;

namespace PensionSystem.Domain.Entities;

public class Contribution : BaseEntity
{
    public Guid MemberId { get; private set; }
    public ContributionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime ContributionDate { get; private set; }
    public string ReferenceNumber { get; private set; } = string.Empty;

    public Member Member { get; private set; } = null!;

    private Contribution() { }

    public Contribution(Guid memberId, ContributionType type, decimal amount, DateTime contributionDate, string referenceNumber)
    {
        Id = Guid.NewGuid();
        MemberId = memberId;
        Type = type;
        Amount = amount;
        ContributionDate = contributionDate;
        ReferenceNumber = referenceNumber;
    }
}
