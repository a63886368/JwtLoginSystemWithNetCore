using System.Threading.Tasks;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Interfaces;
using ManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ManagementSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IUserRepository? _users;
        private IRepository<Role>? _roles;
        private IMenuRepository? _menus;
        private IRepository<UserRole>? _userRoles;
        private IRepository<RoleMenu>? _roleMenus;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IRepository<Role> Roles => _roles ??= new Repository<Role>(_context);
        public IMenuRepository Menus => _menus ??= new MenuRepository(_context);
        public IRepository<UserRole> UserRoles => _userRoles ??= new Repository<UserRole>(_context);
        public IRepository<RoleMenu> RoleMenus => _roleMenus ??= new Repository<RoleMenu>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

