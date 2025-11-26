using System.Collections.Generic;

namespace ManagementSystem.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}

