using FluentValidation.TestHelper;
using PensionSystem.Application.Members.Commands;
using PensionSystem.Application.Members.Validators;
using Xunit;

namespace PensionSystem.Tests.Application;

public class CreateMemberCommandValidatorTests
{
    private readonly CreateMemberCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Age_Under_18()
    {
        // Arrange
        var dob = DateTime.Today.AddYears(-17);
        var command = new CreateMemberCommand("John", "Doe", dob, "test@test.com", "1234567890");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
              .WithErrorMessage("Age must be between 18 and 70 years.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Invalid()
    {
        // Arrange
        var dob = DateTime.Today.AddYears(-30);
        var command = new CreateMemberCommand("John", "Doe", dob, "invalid-email", "1234567890");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        // Arrange
        var dob = DateTime.Today.AddYears(-30);
        var command = new CreateMemberCommand("John", "Doe", dob, "test@example.com", "1234567890");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
