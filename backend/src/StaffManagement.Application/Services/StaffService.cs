using AutoMapper;
using StaffManagement.Application.DTOs;
using StaffManagement.Application.Interfaces;
using StaffManagement.Domain.Entities;
using StaffManagement.Domain.Enums;
using StaffManagement.Domain.Interfaces;

namespace StaffManagement.Application.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _repository;
    private readonly IMapper _mapper;

    public StaffService(IStaffRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<StaffDto?> GetByIdAsync(string id)
    {
        var staff = await _repository.GetByIdAsync(id);
        return staff is null ? null : _mapper.Map<StaffDto>(staff);
    }

    public async Task<PagedResultDto<StaffDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize);
        return new PagedResultDto<StaffDto>
        {
            Items = _mapper.Map<IEnumerable<StaffDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResultDto<StaffDto>> SearchAsync(StaffSearchDto searchDto)
    {
        var (items, totalCount) = await _repository.SearchAsync(
            searchDto.StaffId, searchDto.FullName, searchDto.Gender,
            searchDto.BirthdayFrom, searchDto.BirthdayTo,
            searchDto.Page, searchDto.PageSize);

        return new PagedResultDto<StaffDto>
        {
            Items = _mapper.Map<IEnumerable<StaffDto>>(items),
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };
    }

    public async Task<StaffDto> CreateAsync(CreateStaffDto dto)
    {
        if (await _repository.ExistsAsync(dto.Id))
            throw new InvalidOperationException($"Staff with ID '{dto.Id}' already exists.");

        var staff = _mapper.Map<Staff>(dto);
        var created = await _repository.AddAsync(staff);
        return _mapper.Map<StaffDto>(created);
    }

    public async Task<StaffDto> UpdateAsync(string id, UpdateStaffDto dto)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Staff with ID '{id}' not found.");

        _mapper.Map(dto, existing);
        var updated = await _repository.UpdateAsync(existing);
        return _mapper.Map<StaffDto>(updated);
    }

    public async Task DeleteAsync(string id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException($"Staff with ID '{id}' not found.");

        await _repository.DeleteAsync(id);
    }
}
