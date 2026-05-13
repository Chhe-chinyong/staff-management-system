namespace StaffManagement.Application.DTOs;

public record StaffDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public int Gender { get; set; }
    public string GenderName { get; set; } = string.Empty;
}
