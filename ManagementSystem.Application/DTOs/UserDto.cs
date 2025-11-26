using System;
using System.Collections.Generic;

namespace ManagementSystem.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateTime { get; set; }
    public List<string> Roles { get; set; } = new();
}

