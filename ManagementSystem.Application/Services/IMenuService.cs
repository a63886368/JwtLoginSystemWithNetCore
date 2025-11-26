using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;

namespace ManagementSystem.Application.Services;

public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetMenusByUserIdAsync(int userId);
    Task<IEnumerable<MenuDto>> GetAllMenusAsync();
    Task<MenuDto?> GetMenuByIdAsync(int id);
    Task<MenuDto> CreateMenuAsync(MenuDto menuDto);
    Task<MenuDto?> UpdateMenuAsync(int id, MenuDto menuDto);
    Task<bool> DeleteMenuAsync(int id);
}

