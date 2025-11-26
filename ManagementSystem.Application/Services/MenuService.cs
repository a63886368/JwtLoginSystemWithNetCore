using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Interfaces;

namespace ManagementSystem.Application.Services;

public class MenuService : IMenuService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMenuRepository _menuRepository;

    public MenuService(IUnitOfWork unitOfWork, IMenuRepository menuRepository)
    {
        _unitOfWork = unitOfWork;
        _menuRepository = menuRepository;
    }

    public async Task<IEnumerable<MenuDto>> GetMenusByUserIdAsync(int userId)
    {
        var menus = await _menuRepository.GetMenusByUserIdAsync(userId);
        return BuildMenuTree(menus, null);
    }

    public async Task<IEnumerable<MenuDto>> GetAllMenusAsync()
    {
        var menus = await _menuRepository.GetMenuTreeAsync();
        return menus.Select(MapToDto);
    }

    public async Task<MenuDto?> GetMenuByIdAsync(int id)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(id);
        return menu == null ? null : MapToDto(menu);
    }

    public async Task<MenuDto> CreateMenuAsync(MenuDto menuDto)
    {
        var menu = new Menu
        {
            MenuName = menuDto.MenuName,
            MenuCode = menuDto.MenuCode,
            Path = menuDto.Path,
            Icon = menuDto.Icon,
            ParentId = menuDto.ParentId,
            SortOrder = menuDto.SortOrder,
            IsVisible = menuDto.IsVisible,
            CreateTime = DateTime.Now
        };

        await _unitOfWork.Menus.AddAsync(menu);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(menu);
    }

    public async Task<MenuDto?> UpdateMenuAsync(int id, MenuDto menuDto)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(id);
        if (menu == null) return null;

        menu.MenuName = menuDto.MenuName;
        menu.MenuCode = menuDto.MenuCode;
        menu.Path = menuDto.Path;
        menu.Icon = menuDto.Icon;
        menu.ParentId = menuDto.ParentId;
        menu.SortOrder = menuDto.SortOrder;
        menu.IsVisible = menuDto.IsVisible;
        menu.UpdateTime = DateTime.Now;

        await _unitOfWork.Menus.UpdateAsync(menu);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(menu);
    }

    public async Task<bool> DeleteMenuAsync(int id)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(id);
        if (menu == null) return false;

        await _unitOfWork.Menus.DeleteAsync(menu);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private MenuDto MapToDto(Menu menu)
    {
        return new MenuDto
        {
            Id = menu.Id,
            MenuName = menu.MenuName,
            MenuCode = menu.MenuCode,
            Path = menu.Path,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            SortOrder = menu.SortOrder,
            IsVisible = menu.IsVisible,
            Children = menu.Children?.Select(MapToDto).ToList() ?? new List<MenuDto>()
        };
    }

    private IEnumerable<MenuDto> BuildMenuTree(IEnumerable<Menu> menus, int? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .Select(m => new MenuDto
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

