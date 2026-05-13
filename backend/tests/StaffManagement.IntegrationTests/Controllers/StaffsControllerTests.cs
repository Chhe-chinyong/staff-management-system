using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StaffManagement.Application.DTOs;
using StaffManagement.Infrastructure.Data;
using Xunit;

namespace StaffManagement.IntegrationTests.Controllers;

public class StaffsControllerTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _dbName;

    public StaffsControllerTests()
    {
        _dbName = "TestDb_" + Guid.NewGuid();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });
            });
        });
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private static CreateStaffDto CreateValidCreateDto(string id = "STF00001") => new()
    {
        Id = id,
        FullName = "John Doe",
        Birthday = new DateTime(1990, 1, 15),
        Gender = 1
    };

    [Fact]
    public async Task GetAll_ReturnsOkWithPagedResult()
    {
        var response = await _client.GetAsync("/api/staffs");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ValidStaff_ReturnsCreated()
    {
        var dto = CreateValidCreateDto("CREATE01");
        var response = await _client.PostAsJsonAsync("/api/staffs", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<StaffDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be("CREATE01");
        result.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Create_DuplicateId_ReturnsConflict()
    {
        var dto = CreateValidCreateDto("DUP00001");
        var first = await _client.PostAsJsonAsync("/api/staffs", dto);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.PostAsJsonAsync("/api/staffs", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetById_ExistingStaff_ReturnsOk()
    {
        var dto = CreateValidCreateDto("GETB0001");
        var createResp = await _client.PostAsJsonAsync("/api/staffs", dto);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync("/api/staffs/GETB0001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<StaffDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be("GETB0001");
    }

    [Fact]
    public async Task GetById_NonExistingStaff_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/staffs/NOTFOUND");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ExistingStaff_ReturnsOk()
    {
        var createDto = CreateValidCreateDto("UPDT0001");
        await _client.PostAsJsonAsync("/api/staffs", createDto);

        var updateDto = new UpdateStaffDto
        {
            FullName = "Updated Name",
            Birthday = new DateTime(1991, 6, 20),
            Gender = 2
        };

        var response = await _client.PutAsJsonAsync("/api/staffs/UPDT0001", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<StaffDto>();
        result!.FullName.Should().Be("Updated Name");
        result.GenderName.Should().Be("Female");
    }

    [Fact]
    public async Task Update_NonExistingStaff_ReturnsNotFound()
    {
        var updateDto = new UpdateStaffDto
        {
            FullName = "Name", Birthday = new DateTime(1990, 1, 1), Gender = 1
        };

        var response = await _client.PutAsJsonAsync("/api/staffs/NOTFOUND", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingStaff_ReturnsNoContent()
    {
        var dto = CreateValidCreateDto("DEL00001");
        await _client.PostAsJsonAsync("/api/staffs", dto);

        var response = await _client.DeleteAsync("/api/staffs/DEL00001");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/api/staffs/DEL00001");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistingStaff_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/staffs/NOTFOUND");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Search_WithGenderFilter_ReturnsFilteredResults()
    {
        await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto
        {
            Id = "SRCHM001", FullName = "Male Staff", Birthday = new DateTime(1990, 1, 1), Gender = 1
        });
        await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto
        {
            Id = "SRCHF001", FullName = "Female Staff", Birthday = new DateTime(1995, 1, 1), Gender = 2
        });

        var response = await _client.GetAsync("/api/staffs/search?gender=1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.All(s => s.Gender == 1).Should().BeTrue();
    }

    [Fact]
    public async Task Search_WithStaffIdFilter_ReturnsMatchingResults()
    {
        await _client.PostAsJsonAsync("/api/staffs", CreateValidCreateDto("SRCHID01"));

        var response = await _client.GetAsync("/api/staffs/search?staffId=SRCHID");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.All(s => s.Id.Contains("SRCHID")).Should().BeTrue();
    }

    [Fact]
    public async Task Search_WithBirthdayRange_ReturnsFilteredResults()
    {
        await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto
        {
            Id = "BDAY0001", FullName = "Old Staff", Birthday = new DateTime(1985, 5, 10), Gender = 1
        });
        await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto
        {
            Id = "BDAY0002", FullName = "Young Staff", Birthday = new DateTime(2000, 3, 20), Gender = 1
        });

        var response = await _client.GetAsync("/api/staffs/search?birthdayFrom=1990-01-01&birthdayTo=2010-12-31");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.All(s => s.Birthday >= new DateTime(1990, 1, 1) && s.Birthday <= new DateTime(2010, 12, 31)).Should().BeTrue();
    }
}
