using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
    }
}

