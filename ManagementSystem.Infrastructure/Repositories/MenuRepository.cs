using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Interfaces;
using ManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Infrastructure.Repositories
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Menu>> GetMenusByRoleIdAsync(int roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId)
                .Include(rm => rm.Menu)
                    .ThenInclude(m => m.Children)
                .Select(rm => rm.Menu)
                .Where(m => m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetMenusByUserIdAsync(int userId)
        {
            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            return await _context.RoleMenus
                .Where(rm => roleIds.Contains(rm.RoleId))
                .Include(rm => rm.Menu)
                    .ThenInclude(m => m.Children)
                .Select(rm => rm.Menu)
                .Where(m => m.IsVisible)
                .Distinct()
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetMenuTreeAsync()
        {
            var allMenus = await _dbSet
                .Where(m => m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            return BuildMenuTree(allMenus, null);
        }

        private IEnumerable<Menu> BuildMenuTree(IEnumerable<Menu> menus, int? parentId)
        {
            return menus
                .Where(m => m.ParentId == parentId)
                .Select(m => new Menu
                {
                    Id = m.Id,
                    MenuName = m.MenuName,
                    MenuCode = m.MenuCode,
                    Path = m.Path,
                    Icon = m.Icon,
                    ParentId = m.ParentId,
                    SortOrder = m.SortOrder,
                    IsVisible = m.IsVisible,
                    Children = BuildMenuTree(menus, m.Id).ToList()
                });
        }
    }
}

