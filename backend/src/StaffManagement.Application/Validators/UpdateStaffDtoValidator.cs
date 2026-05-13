using FluentValidation;
using StaffManagement.Domain.Enums;

namespace StaffManagement.Application.Validators;

public class UpdateStaffDtoValidator : AbstractValidator<DTOs.UpdateStaffDto>
{
    public UpdateStaffDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .Length(1, 100).WithMessage("Full name must be between 1 and 100 characters.");

        RuleFor(x => x.Birthday)
            .NotEmpty().WithMessage("Birthday is required.")
            .LessThan(DateTime.Today).WithMessage("Birthday must be in the past.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => Enum.IsDefined(typeof(Gender), g)).WithMessage("Gender must be 1 (Male) or 2 (Female).");
    }
}
