using System.Linq;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Interfaces;
using ManagementSystem.Infrastructure.Services;
using BCrypt.Net;
using System.Collections.Generic;

namespace ManagementSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUserNameAsync(loginDto.UserName);
        if (user == null || !user.IsActive)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return null;
        }

        // 使用已通过 Include 加载的角色数据，避免再次查询导致 DataReader 冲突
        var roles = user.UserRoles?.Select(ur => ur.Role).Where(r => r != null).ToList() ?? new List<Role>();
        var token = _jwtService.GenerateToken(user, roles);

        return new LoginResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                IsActive = user.IsActive,
                CreateTime = user.CreateTime,
                Roles = roles.Select(r => r.RoleName).ToList()
            },
            Roles = roles.Select(r => r.RoleName).ToList()
        };
    }
}

