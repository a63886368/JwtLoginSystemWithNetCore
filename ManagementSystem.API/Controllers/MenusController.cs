using System.Security.Claims;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet("my-menus")]
    public async Task<IActionResult> GetMyMenus()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { message = "无效的用户信息" });
        }

        var menus = await _menuService.GetMenusByUserIdAsync(userId);
        return Ok(menus);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllMenus()
    {
        var menus = await _menuService.GetAllMenusAsync();
        return Ok(menus);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMenuById(int id)
    {
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu == null)
        {
            return NotFound(new { message = "菜单不存在" });
        }
        return Ok(menu);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMenu([FromBody] MenuDto menuDto)
    {
        var menu = await _menuService.CreateMenuAsync(menuDto);
        return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMenu(int id, [FromBody] MenuDto menuDto)
    {
        var menu = await _menuService.UpdateMenuAsync(id, menuDto);
        if (menu == null)
        {
            return NotFound(new { message = "菜单不存在" });
        }
        return Ok(menu);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMenu(int id)
    {
        var result = await _menuService.DeleteMenuAsync(id);
        if (!result)
        {
            return NotFound(new { message = "菜单不存在" });
        }
        return NoContent();
    }
}

