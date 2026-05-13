using Microsoft.EntityFrameworkCore;
using StaffManagement.Domain.Entities;
using StaffManagement.Domain.Enums;
using StaffManagement.Domain.Interfaces;
using StaffManagement.Infrastructure.Data;

namespace StaffManagement.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly AppDbContext _context;

    public StaffRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Staff?> GetByIdAsync(string id)
    {
        return await _context.Staffs.FindAsync(id);
    }

    public async Task<(IEnumerable<Staff> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Staffs.AsQueryable();
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Staff> Items, int TotalCount)> SearchAsync(
        string? staffId, string? fullName, int? gender, DateTime? birthdayFrom, DateTime? birthdayTo,
        int page, int pageSize)
    {
        var query = _context.Staffs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(staffId))
            query = query.Where(s => s.Id.Contains(staffId));

        if (!string.IsNullOrWhiteSpace(fullName))
            query = query.Where(s => s.FullName.Contains(fullName));

        if (gender.HasValue)
            query = query.Where(s => s.Gender == (Gender)gender.Value);

        if (birthdayFrom.HasValue)
            query = query.Where(s => s.Birthday >= birthdayFrom.Value);

        if (birthdayTo.HasValue)
            query = query.Where(s => s.Birthday <= birthdayTo.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Staff> AddAsync(Staff staff)
    {
        _context.Staffs.Add(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<Staff> UpdateAsync(Staff staff)
    {
        _context.Staffs.Update(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task DeleteAsync(string id)
    {
        var staff = await _context.Staffs.FindAsync(id);
        if (staff is not null)
        {
            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Staffs.AnyAsync(s => s.Id == id);
    }
}
