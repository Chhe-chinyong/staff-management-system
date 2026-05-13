using FluentAssertions;
using StaffManagement.Application.DTOs;
using StaffManagement.Application.Validators;

namespace StaffManagement.UnitTests.Validators;

public class StaffSearchDtoValidatorTests
{
    private readonly StaffSearchDtoValidator _validator = new();

    [Fact]
    public void Validate_DefaultValues_ReturnsNoErrors()
    {
        var dto = new StaffSearchDto();
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_StaffIdExceedsMaxLength_ReturnsError()
    {
        var dto = new StaffSearchDto { StaffId = "123456789" };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_InvalidGender_ReturnsError()
    {
        var dto = new StaffSearchDto { Gender = 5 };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_BirthdayToBeforeFrom_ReturnsError()
    {
        var dto = new StaffSearchDto
        {
            BirthdayFrom = new DateTime(2000, 1, 1),
            BirthdayTo = new DateTime(1999, 1, 1)
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_PageLessThanOne_ReturnsError()
    {
        var dto = new StaffSearchDto { Page = 0 };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_PageSizeExceedsMax_ReturnsError()
    {
        var dto = new StaffSearchDto { PageSize = 101 };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ValidSearchCriteria_ReturnsNoErrors()
    {
        var dto = new StaffSearchDto
        {
            StaffId = "STF",
            Gender = 1,
            BirthdayFrom = new DateTime(1990, 1, 1),
            BirthdayTo = new DateTime(2000, 12, 31),
            Page = 1,
            PageSize = 20
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }
}
