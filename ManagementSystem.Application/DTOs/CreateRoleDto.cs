using System.Collections.Generic;

namespace ManagementSystem.Application.DTOs;

public class CreateRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<int>? MenuIds { get; set; }
}

