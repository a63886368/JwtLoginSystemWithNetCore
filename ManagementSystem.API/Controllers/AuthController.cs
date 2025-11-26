using System;
using System.Threading.Tasks;
using ManagementSystem.Application.DTOs;
using ManagementSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    _logger.LogWarning("登录请求：请求体为空");
                    return BadRequest(new { message = "请求数据不能为空" });
                }

                if (string.IsNullOrEmpty(loginDto.UserName) || string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning("登录请求：用户名或密码为空");
                    return BadRequest(new { message = "用户名和密码不能为空" });
                }

                _logger.LogInformation("尝试登录：用户名 = {UserName}", loginDto.UserName);

                var result = await _authService.LoginAsync(loginDto);
                if (result == null)
                {
                    _logger.LogWarning("登录失败：用户名或密码错误 - {UserName}", loginDto.UserName);
                    return Unauthorized(new { message = "用户名或密码错误" });
                }

                _logger.LogInformation("登录成功：用户名 = {UserName}", loginDto.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登录时发生异常：用户名 = {UserName}", loginDto?.UserName ?? "未知");
                return StatusCode(500, new { message = "服务器内部错误" });
            }
        }
    }
}

