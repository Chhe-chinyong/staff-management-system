namespace StaffManagement.Application.DTOs;

public record UpdateStaffDto
{
    public string FullName { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public int Gender { get; set; }
}
