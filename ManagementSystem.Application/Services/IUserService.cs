using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;

namespace ManagementSystem.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(int id);
}

