using System;
using System.Threading.Tasks;
using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRepository<Role> Roles { get; }
        IMenuRepository Menus { get; }
        IRepository<UserRole> UserRoles { get; }
        IRepository<RoleMenu> RoleMenus { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

