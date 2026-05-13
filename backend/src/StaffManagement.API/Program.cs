using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StaffManagement.API.Middleware;
using StaffManagement.Application.Interfaces;
using StaffManagement.Application.Mapping;
using StaffManagement.Application.Services;
using StaffManagement.Domain.Interfaces;
using StaffManagement.Infrastructure.Data;
using StaffManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Staff Management API", Version = "v1" });
});

var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
if (useInMemory)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("StaffManagementDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();

builder.Services.AddAutoMapper(typeof(StaffMappingProfile));

builder.Services.AddValidatorsFromAssembly(Assembly.Load("StaffManagement.Application"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
