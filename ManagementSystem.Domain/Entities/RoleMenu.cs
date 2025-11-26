namespace ManagementSystem.Domain.Entities
{
    public class RoleMenu
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }

        // 导航属性
        public virtual Role Role { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
    }
}

