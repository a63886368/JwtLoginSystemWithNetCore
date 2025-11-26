using System.Collections.Generic;

namespace ManagementSystem.Application.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string? MenuCode { get; set; }
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public List<MenuDto> Children { get; set; } = new();
}

