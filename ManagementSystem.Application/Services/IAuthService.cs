using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;

namespace ManagementSystem.Application.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
}

