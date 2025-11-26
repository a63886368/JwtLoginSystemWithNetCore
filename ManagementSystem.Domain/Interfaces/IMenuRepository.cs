using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Domain.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<IEnumerable<Menu>> GetMenusByRoleIdAsync(int roleId);
        Task<IEnumerable<Menu>> GetMenusByUserIdAsync(int userId);
        Task<IEnumerable<Menu>> GetMenuTreeAsync();
    }
}

