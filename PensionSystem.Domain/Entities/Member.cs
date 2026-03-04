using PensionSystem.Domain.Common;

namespace PensionSystem.Domain.Entities;

public class Member : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    
    // Navigation Properties
    private readonly List<Contribution> _contributions = new();
    public IReadOnlyCollection<Contribution> Contributions => _contributions.AsReadOnly();

    private readonly List<Benefit> _benefits = new();
    public IReadOnlyCollection<Benefit> Benefits => _benefits.AsReadOnly();

    private Member() { } // EF Core

    public Member(string firstName, string lastName, DateTime dateOfBirth, string email, string phoneNumber)
    {
        Id = Guid.NewGuid();
        UpdateDetails(firstName, lastName, dateOfBirth, email, phoneNumber);
    }

    public void UpdateDetails(string firstName, string lastName, DateTime dateOfBirth, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Email = email;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}
