using System.Collections.Generic;

namespace ManagementSystem.Application.DTOs;

public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool? IsActive { get; set; }
    public List<int>? RoleIds { get; set; }
}

