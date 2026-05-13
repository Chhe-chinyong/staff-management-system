namespace StaffManagement.Application.DTOs;

public record StaffSearchDto
{
    public string? StaffId { get; set; }
    public string? FullName { get; set; }
    public int? Gender { get; set; }
    public DateTime? BirthdayFrom { get; set; }
    public DateTime? BirthdayTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
