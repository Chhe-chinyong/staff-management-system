using FluentAssertions;
using StaffManagement.Application.DTOs;
using StaffManagement.Application.Validators;

namespace StaffManagement.UnitTests.Validators;

public class CreateStaffDtoValidatorTests
{
    private readonly CreateStaffDtoValidator _validator = new();

    private static CreateStaffDto CreateValidDto() => new()
    {
        Id = "STF00001",
        FullName = "John Doe",
        Birthday = new DateTime(1990, 1, 15),
        Gender = 1
    };

    [Fact]
    public void Validate_ValidDto_ReturnsNoErrors()
    {
        var dto = CreateValidDto();
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyId_ReturnsError()
    {
        var dto = CreateValidDto() with { Id = "" };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_IdExceedsMaxLength_ReturnsError()
    {
        var dto = CreateValidDto() with { Id = "123456789" };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_EmptyFullName_ReturnsError()
    {
        var dto = CreateValidDto() with { FullName = "" };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_FullNameExceedsMaxLength_ReturnsError()
    {
        var dto = CreateValidDto() with { FullName = new string('a', 101) };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_FutureBirthday_ReturnsError()
    {
        var dto = CreateValidDto() with { Birthday = DateTime.Today.AddDays(1) };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Birthday");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void Validate_InvalidGender_ReturnsError(int gender)
    {
        var dto = CreateValidDto() with { Gender = gender };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Gender");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Validate_ValidGender_ReturnsNoError(int gender)
    {
        var dto = CreateValidDto() with { Gender = gender };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }
}
