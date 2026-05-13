using FluentValidation;
using StaffManagement.Domain.Enums;

namespace StaffManagement.Application.Validators;

public class StaffSearchDtoValidator : AbstractValidator<DTOs.StaffSearchDto>
{
    public StaffSearchDtoValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.FullName))
            .WithMessage("Full name must not exceed 100 characters.");


        RuleFor(x => x.StaffId)
            .MaximumLength(8).When(x => !string.IsNullOrEmpty(x.StaffId))
            .WithMessage("Staff ID must not exceed 8 characters.");

        RuleFor(x => x.Gender)
            .Must(g => g == null || Enum.IsDefined(typeof(Gender), g.Value))
            .WithMessage("Gender must be 1 (Male) or 2 (Female).");

        RuleFor(x => x.BirthdayTo)
            .GreaterThanOrEqualTo(x => x.BirthdayFrom)
            .When(x => x.BirthdayFrom.HasValue && x.BirthdayTo.HasValue)
            .WithMessage("BirthdayTo must be greater than or equal to BirthdayFrom.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
