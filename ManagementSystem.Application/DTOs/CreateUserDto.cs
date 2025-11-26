using System.Collections.Generic;

namespace ManagementSystem.Application.DTOs;

public class CreateUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public List<int> RoleIds { get; set; } = new();
}

