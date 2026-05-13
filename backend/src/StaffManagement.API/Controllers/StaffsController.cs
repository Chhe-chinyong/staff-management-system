using Microsoft.AspNetCore.Mvc;
using StaffManagement.Application.DTOs;
using StaffManagement.Application.Interfaces;

namespace StaffManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffsController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffsController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<StaffDto>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _staffService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StaffDto>> GetById(string id)
    {
        var staff = await _staffService.GetByIdAsync(id);
        if (staff is null) return NotFound(new { message = $"Staff with ID '{id}' not found." });
        return Ok(staff);
    }

    [HttpPost]
    public async Task<ActionResult<StaffDto>> Create([FromBody] CreateStaffDto dto)
    {
        var staff = await _staffService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = staff.Id }, staff);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StaffDto>> Update(string id, [FromBody] UpdateStaffDto dto)
    {
        var staff = await _staffService.UpdateAsync(id, dto);
        return Ok(staff);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _staffService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResultDto<StaffDto>>> Search(
        [FromQuery] StaffSearchDto searchDto)
    {
        var result = await _staffService.SearchAsync(searchDto);
        return Ok(result);
    }
}
