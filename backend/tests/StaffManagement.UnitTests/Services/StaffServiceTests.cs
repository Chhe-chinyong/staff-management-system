using AutoMapper;
using FluentAssertions;
using Moq;
using StaffManagement.Application.DTOs;
using StaffManagement.Application.Mapping;
using StaffManagement.Application.Services;
using StaffManagement.Domain.Entities;
using StaffManagement.Domain.Enums;
using StaffManagement.Domain.Interfaces;

namespace StaffManagement.UnitTests.Services;

public class StaffServiceTests
{
    private readonly Mock<IStaffRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly StaffService _service;

    public StaffServiceTests()
    {
        _mockRepo = new Mock<IStaffRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<StaffMappingProfile>());
        _mapper = config.CreateMapper();
        _service = new StaffService(_mockRepo.Object, _mapper);
    }

    private static Staff CreateSampleStaff(string id = "STF00001") => new()
    {
        Id = id,
        FullName = "John Doe",
        Birthday = new DateTime(1990, 1, 15),
        Gender = Gender.Male
    };

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsStaffDto()
    {
        var staff = CreateSampleStaff();
        _mockRepo.Setup(r => r.GetByIdAsync("STF00001")).ReturnsAsync(staff);

        var result = await _service.GetByIdAsync("STF00001");

        result.Should().NotBeNull();
        result!.Id.Should().Be("STF00001");
        result.FullName.Should().Be("John Doe");
        result.GenderName.Should().Be("Male");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync("NOTFOUND")).ReturnsAsync((Staff?)null);

        var result = await _service.GetByIdAsync("NOTFOUND");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var staffs = new List<Staff> { CreateSampleStaff("STF00001"), CreateSampleStaff("STF00002") };
        _mockRepo.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync((staffs, 2));

        var result = await _service.GetAllAsync(1, 10);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_CreatesAndReturnsStaff()
    {
        var dto = new CreateStaffDto
        {
            Id = "STF00001", FullName = "Jane Doe",
            Birthday = new DateTime(1995, 5, 20), Gender = 2
        };
        _mockRepo.Setup(r => r.ExistsAsync("STF00001")).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Staff>()))
            .ReturnsAsync((Staff s) => s);

        var result = await _service.CreateAsync(dto);

        result.Id.Should().Be("STF00001");
        result.FullName.Should().Be("Jane Doe");
        result.GenderName.Should().Be("Female");
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ThrowsInvalidOperationException()
    {
        var dto = new CreateStaffDto { Id = "STF00001", FullName = "John", Birthday = DateTime.Today.AddYears(-25), Gender = 1 };
        _mockRepo.Setup(r => r.ExistsAsync("STF00001")).ReturnsAsync(true);

        var act = () => _service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task UpdateAsync_ExistingStaff_UpdatesAndReturns()
    {
        var existing = CreateSampleStaff();
        var dto = new UpdateStaffDto { FullName = "Updated Name", Birthday = new DateTime(1991, 3, 10), Gender = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync("STF00001")).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Staff>()))
            .ReturnsAsync((Staff s) => s);

        var result = await _service.UpdateAsync("STF00001", dto);

        result.FullName.Should().Be("Updated Name");
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Staff>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingStaff_ThrowsKeyNotFoundException()
    {
        var dto = new UpdateStaffDto { FullName = "Name", Birthday = DateTime.Today.AddYears(-25), Gender = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync("NOTFOUND")).ReturnsAsync((Staff?)null);

        var act = () => _service.UpdateAsync("NOTFOUND", dto);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task DeleteAsync_ExistingStaff_DeletesSuccessfully()
    {
        _mockRepo.Setup(r => r.ExistsAsync("STF00001")).ReturnsAsync(true);
        _mockRepo.Setup(r => r.DeleteAsync("STF00001")).Returns(Task.CompletedTask);

        await _service.DeleteAsync("STF00001");

        _mockRepo.Verify(r => r.DeleteAsync("STF00001"), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingStaff_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.ExistsAsync("NOTFOUND")).ReturnsAsync(false);

        var act = () => _service.DeleteAsync("NOTFOUND");

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task SearchAsync_WithFilters_ReturnsFilteredResults()
    {
        var searchDto = new StaffSearchDto { Gender = 1, Page = 1, PageSize = 10 };
        var staffs = new List<Staff> { CreateSampleStaff() };
        _mockRepo.Setup(r => r.SearchAsync(null, null, 1, null, null, 1, 10))
            .ReturnsAsync((staffs, 1));

        var result = await _service.SearchAsync(searchDto);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task SearchAsync_WithBirthdayRange_ReturnsResults()
    {
        var from = new DateTime(1990, 1, 1);
        var to = new DateTime(1995, 12, 31);
        var searchDto = new StaffSearchDto { BirthdayFrom = from, BirthdayTo = to, Page = 1, PageSize = 10 };
        var staffs = new List<Staff> { CreateSampleStaff() };
        _mockRepo.Setup(r => r.SearchAsync(null, null, null, from, to, 1, 10))
            .ReturnsAsync((staffs, 1));

        var result = await _service.SearchAsync(searchDto);

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        _mockRepo.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync((new List<Staff>(), 0));

        var result = await _service.GetAllAsync(1, 10);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}
