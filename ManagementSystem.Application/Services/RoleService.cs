using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Interfaces;

namespace ManagementSystem.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(id);
        return role == null ? null : MapToDto(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();
        return roles.Select(MapToDto);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        // 检查角色名是否已存在
        var existingRole = await _unitOfWork.Roles.FindAsync(r => r.RoleName == createRoleDto.RoleName);
        if (existingRole.Any())
        {
            throw new Exception("角色名已存在");
        }

        var role = new Role
        {
            RoleName = createRoleDto.RoleName,
            Description = createRoleDto.Description,
            IsActive = true,
            CreateTime = DateTime.Now
        };

        await _unitOfWork.Roles.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        // 添加角色菜单关联
        if (createRoleDto.MenuIds != null && createRoleDto.MenuIds.Any())
        {
            foreach (var menuId in createRoleDto.MenuIds)
            {
                var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);
                if (menu != null)
                {
                    await _unitOfWork.RoleMenus.AddAsync(new RoleMenu
                    {
                        RoleId = role.Id,
                        MenuId = menuId
                    });
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        return MapToDto(role);
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, CreateRoleDto updateRoleDto)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(id);
        if (role == null) return null;

        // 检查角色名是否已被其他角色使用
        var existingRole = await _unitOfWork.Roles.FindAsync(r => r.RoleName == updateRoleDto.RoleName && r.Id != id);
        if (existingRole.Any())
        {
            throw new Exception("角色名已存在");
        }

        role.RoleName = updateRoleDto.RoleName;
        role.Description = updateRoleDto.Description;
        role.UpdateTime = DateTime.Now;

        await _unitOfWork.Roles.UpdateAsync(role);

        // 更新角色菜单关联
        if (updateRoleDto.MenuIds != null)
        {
            var existingRoleMenus = await _unitOfWork.RoleMenus.FindAsync(rm => rm.RoleId == id);
            foreach (var roleMenu in existingRoleMenus)
            {
                await _unitOfWork.RoleMenus.DeleteAsync(roleMenu);
            }

            foreach (var menuId in updateRoleDto.MenuIds)
            {
                var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);
                if (menu != null)
                {
                    await _unitOfWork.RoleMenus.AddAsync(new RoleMenu
                    {
                        RoleId = id,
                        MenuId = menuId
                    });
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return MapToDto(role);
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(id);
        if (role == null) return false;

        await _unitOfWork.Roles.DeleteAsync(role);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
            Description = role.Description,
            IsActive = role.IsActive,
            CreateTime = role.CreateTime
        };
    }
}

