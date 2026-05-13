using StaffManagement.Application.DTOs;

namespace StaffManagement.Application.Interfaces;

public interface IStaffService
{
    Task<StaffDto?> GetByIdAsync(string id);
    Task<PagedResultDto<StaffDto>> GetAllAsync(int page, int pageSize);
    Task<PagedResultDto<StaffDto>> SearchAsync(StaffSearchDto searchDto);
    Task<StaffDto> CreateAsync(CreateStaffDto dto);
    Task<StaffDto> UpdateAsync(string id, UpdateStaffDto dto);
    Task DeleteAsync(string id);
}
