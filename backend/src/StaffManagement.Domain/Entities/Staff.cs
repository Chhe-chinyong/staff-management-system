using StaffManagement.Domain.Enums;

namespace StaffManagement.Domain.Entities;

public class Staff
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public Gender Gender { get; set; }
}
