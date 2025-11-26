using System;
using System.Collections.Generic;

namespace ManagementSystem.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? MenuCode { get; set; }
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? UpdateTime { get; set; }

        // 导航属性
        public virtual Menu? Parent { get; set; }
        public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();
        public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }
}

