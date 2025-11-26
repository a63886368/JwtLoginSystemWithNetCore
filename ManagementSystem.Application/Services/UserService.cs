using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Interfaces;
using BCrypt.Net;

namespace ManagementSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return null;

        var roles = await _unitOfWork.Users.GetUserRolesAsync(id);
        return MapToDto(user, roles);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        // 先获取所有用户
        var users = await _unitOfWork.Users.GetAllAsync();
        var userList = users.ToList();
        
        if (!userList.Any())
        {
            return new List<UserDto>();
        }

        // 一次性查询所有用户的角色，避免在循环中多次查询导致 DataReader 冲突
        var userIds = userList.Select(u => u.Id).ToList();
        
        // 批量查询所有用户角色和角色信息
        var allUserRoles = (await _unitOfWork.UserRoles.FindAsync(ur => userIds.Contains(ur.UserId))).ToList();
        var allRoles = (await _unitOfWork.Roles.GetAllAsync()).ToList();
        
        // 构建角色ID到角色的映射，提高查找效率
        var rolesDict = allRoles.ToDictionary(r => r.Id);
        
        // 构建用户ID到角色列表的映射
        var userRolesDict = allUserRoles
            .GroupBy(ur => ur.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(ur => rolesDict.GetValueOrDefault(ur.RoleId))
                      .Where(r => r != null)
                      .Cast<Role>()
                      .ToList()
            );

        // 映射到 DTO
        var userDtos = new List<UserDto>();
        foreach (var user in userList)
        {
            var roles = userRolesDict.GetValueOrDefault(user.Id, new List<Role>());
            userDtos.Add(MapToDto(user, roles));
        }

        return userDtos;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // 检查用户名是否已存在
        var existingUser = await _unitOfWork.Users.GetByUserNameAsync(createUserDto.UserName);
        if (existingUser != null)
        {
            throw new Exception("用户名已存在");
        }

        // 检查邮箱是否已存在
        var existingEmail = await ((IUserRepository)_unitOfWork.Users).GetByEmailAsync(createUserDto.Email);
        if (existingEmail != null)
        {
            throw new Exception("邮箱已存在");
        }

        var user = new User
        {
            UserName = createUserDto.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            Email = createUserDto.Email,
            Phone = createUserDto.Phone,
            IsActive = true,
            CreateTime = DateTime.Now
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // 添加用户角色
        if (createUserDto.RoleIds.Any())
        {
            foreach (var roleId in createUserDto.RoleIds)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role != null)
                {
                    await _unitOfWork.UserRoles.AddAsync(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    });
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        var roles = await _unitOfWork.Users.GetUserRolesAsync(user.Id);
        return MapToDto(user, roles);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(updateUserDto.Email))
        {
            var existingEmail = await ((IUserRepository)_unitOfWork.Users).GetByEmailAsync(updateUserDto.Email);
            if (existingEmail != null && existingEmail.Id != id)
            {
                throw new Exception("邮箱已存在");
            }
            user.Email = updateUserDto.Email;
        }

        if (!string.IsNullOrEmpty(updateUserDto.Phone))
        {
            user.Phone = updateUserDto.Phone;
        }

        if (updateUserDto.IsActive.HasValue)
        {
            user.IsActive = updateUserDto.IsActive.Value;
        }

        user.UpdateTime = DateTime.Now;

        await _unitOfWork.Users.UpdateAsync(user);

        // 更新用户角色
        if (updateUserDto.RoleIds != null)
        {
            var existingUserRoles = await _unitOfWork.UserRoles.FindAsync(ur => ur.UserId == id);
            foreach (var userRole in existingUserRoles)
            {
                await _unitOfWork.UserRoles.DeleteAsync(userRole);
            }

            foreach (var roleId in updateUserDto.RoleIds)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role != null)
                {
                    await _unitOfWork.UserRoles.AddAsync(new UserRole
                    {
                        UserId = id,
                        RoleId = roleId
                    });
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var roles = await _unitOfWork.Users.GetUserRolesAsync(id);
        return MapToDto(user, roles);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return false;

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private UserDto MapToDto(User user, IEnumerable<Role> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone,
            IsActive = user.IsActive,
            CreateTime = user.CreateTime,
            Roles = roles.Select(r => r.RoleName).ToList()
        };
    }
}

