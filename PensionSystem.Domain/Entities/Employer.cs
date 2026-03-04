using PensionSystem.Domain.Common;

namespace PensionSystem.Domain.Entities;

public class Employer : BaseEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string RegistrationNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Employer() { }

    public Employer(string companyName, string registrationNumber)
    {
        Id = Guid.NewGuid();
        CompanyName = companyName;
        RegistrationNumber = registrationNumber;
        IsActive = true;
    }

    public void UpdateStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
