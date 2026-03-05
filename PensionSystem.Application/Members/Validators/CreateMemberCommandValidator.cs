using FluentValidation;
using PensionSystem.Application.Members.Commands;

namespace PensionSystem.Application.Members.Validators;

public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(BeAValidAge)
            .WithMessage("Age must be between 18 and 70 years.");
    }

    private bool BeAValidAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        
        return age >= 18 && age <= 70;
    }
}
