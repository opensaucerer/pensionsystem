using FluentValidation;
using PensionSystem.Application.Contributions.Commands;

namespace PensionSystem.Application.Contributions.Validators;

public class AddContributionCommandValidator : AbstractValidator<AddContributionCommand>
{
    public AddContributionCommandValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.ContributionDate).NotEmpty();
        RuleFor(x => x.ReferenceNumber).NotEmpty().MaximumLength(100);
    }
}
