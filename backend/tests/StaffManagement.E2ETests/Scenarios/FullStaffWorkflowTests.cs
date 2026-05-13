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

namespace StaffManagement.E2ETests.Scenarios;

public class FullStaffWorkflowTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public FullStaffWorkflowTests()
    {
        var dbName = "E2ETestDb_" + Guid.NewGuid();
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
                    options.UseInMemoryDatabase(dbName);
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

    [Fact]
    public async Task FullWorkflow_Create_Read_Update_Search_Delete()
    {
        var createDto = new CreateStaffDto
        {
            Id = "E2E00001",
            FullName = "Workflow Test",
            Birthday = new DateTime(1992, 7, 10),
            Gender = 1
        };

        var createResponse = await _client.PostAsJsonAsync("/api/staffs", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<StaffDto>();
        created!.Id.Should().Be("E2E00001");

        var getResponse = await _client.GetAsync("/api/staffs/E2E00001");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<StaffDto>();
        fetched!.Id.Should().Be("E2E00001");

        var updateDto = new UpdateStaffDto
        {
            FullName = "Updated Workflow",
            Birthday = new DateTime(1993, 8, 15),
            Gender = 2
        };
        var updateResponse = await _client.PutAsJsonAsync("/api/staffs/E2E00001", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<StaffDto>();
        updated!.FullName.Should().Be("Updated Workflow");

        var searchResponse = await _client.GetAsync("/api/staffs/search?staffId=E2E");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        searchResult!.Items.Should().ContainSingle(s => s.Id == "E2E00001");

        var deleteResponse = await _client.DeleteAsync("/api/staffs/E2E00001");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifyResponse = await _client.GetAsync("/api/staffs/E2E00001");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MultiStaffWorkflow_SearchByGenderAndBirthday()
    {
        await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto { Id = "E2E-M001", FullName = "Male Staff", Birthday = new DateTime(1988, 3, 15), Gender = 1 });
        await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto { Id = "E2E-F001", FullName = "Female Staff", Birthday = new DateTime(1995, 7, 20), Gender = 2 });
        await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto { Id = "E2E-M002", FullName = "Male Staff 2", Birthday = new DateTime(1999, 1, 5), Gender = 1 });

        var maleResponse = await _client.GetAsync("/api/staffs/search?gender=1");
        maleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var maleResult = await maleResponse.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        maleResult!.Items.Should().NotBeEmpty();
        maleResult.Items.All(s => s.Gender == 1).Should().BeTrue();
    }

    [Fact]
    public async Task PaginationWorkflow_VerifyPagingWorksCorrectly()
    {
        for (int i = 1; i <= 15; i++)
        {
            await _client.PostAsJsonAsync("/api/staffs", new CreateStaffDto
            {
                Id = $"PAGE{i:D4}",
                FullName = $"Staff {i}",
                Birthday = new DateTime(1990, 1, 1).AddDays(i),
                Gender = i % 2 == 0 ? 2 : 1
            });
        }

        var page1Response = await _client.GetAsync("/api/staffs?page=1&pageSize=5");
        page1Response.StatusCode.Should().Be(HttpStatusCode.OK);
        var page1 = await page1Response.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        page1!.Items.Should().HaveCount(5);
        page1.TotalCount.Should().Be(15);
        page1.HasNextPage.Should().BeTrue();

        var page2Response = await _client.GetAsync("/api/staffs?page=2&pageSize=5");
        page2Response.StatusCode.Should().Be(HttpStatusCode.OK);
        var page2 = await page2Response.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        page2!.Items.Should().HaveCount(5);
        page2.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public async Task ErrorHandling_DuplicateCreate_ReturnsProperError()
    {
        var dto = new CreateStaffDto { Id = "ERRO0001", FullName = "Test", Birthday = new DateTime(1990, 1, 1), Gender = 1 };
        var first = await _client.PostAsJsonAsync("/api/staffs", dto);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var duplicateResponse = await _client.PostAsJsonAsync("/api/staffs", dto);
        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ErrorHandling_UpdateNonExistent_ReturnsNotFound()
    {
        var updateDto = new UpdateStaffDto { FullName = "Ghost", Birthday = new DateTime(1990, 1, 1), Gender = 1 };
        var response = await _client.PutAsJsonAsync("/api/staffs/GHOST01", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Search_EmptyResults_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/api/staffs/search?staffId=NONEXISTENT");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<StaffDto>>();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
