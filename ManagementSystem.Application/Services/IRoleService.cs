using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;

namespace ManagementSystem.Application.Services;

public interface IRoleService
{
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<RoleDto?> UpdateRoleAsync(int id, CreateRoleDto updateRoleDto);
    Task<bool> DeleteRoleAsync(int id);
}

