using FluentAssertions;
using PensionSystem.Domain.Entities;
using Xunit;

namespace PensionSystem.Tests.Domain;

public class MemberTests
{
    [Fact]
    public void Should_Calculate_Correct_Age()
    {
        // Arrange
        var dob = DateTime.Today.AddYears(-25);
        var member = new Member("John", "Doe", dob, "john@example.com", "1234567890");

        // Act
        var age = member.GetAge();

        // Assert
        age.Should().Be(25);
    }
}
