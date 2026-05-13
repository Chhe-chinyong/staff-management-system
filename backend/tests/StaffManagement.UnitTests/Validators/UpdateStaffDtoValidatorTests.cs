using FluentAssertions;
using StaffManagement.Application.DTOs;
using StaffManagement.Application.Validators;

namespace StaffManagement.UnitTests.Validators;

public class UpdateStaffDtoValidatorTests
{
    private readonly UpdateStaffDtoValidator _validator = new();

    private static UpdateStaffDto CreateValidDto() => new()
    {
        FullName = "John Doe",
        Birthday = new DateTime(1990, 1, 15),
        Gender = 1
    };

    [Fact]
    public void Validate_ValidDto_ReturnsNoErrors()
    {
        var result = _validator.Validate(CreateValidDto());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyFullName_ReturnsError()
    {
        var dto = CreateValidDto() with { FullName = "" };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_FutureBirthday_ReturnsError()
    {
        var dto = CreateValidDto() with { Birthday = DateTime.Today.AddDays(1) };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void Validate_InvalidGender_ReturnsError(int gender)
    {
        var dto = CreateValidDto() with { Gender = gender };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }
}
