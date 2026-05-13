using StaffManagement.Domain.Entities;

namespace StaffManagement.Domain.Interfaces;

public interface IStaffRepository
{
    Task<Staff?> GetByIdAsync(string id);
    Task<(IEnumerable<Staff> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<(IEnumerable<Staff> Items, int TotalCount)> SearchAsync(
        string? staffId, string? fullName, int? gender, DateTime? birthdayFrom, DateTime? birthdayTo,
        int page, int pageSize);
    Task<Staff> AddAsync(Staff staff);
    Task<Staff> UpdateAsync(Staff staff);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}
