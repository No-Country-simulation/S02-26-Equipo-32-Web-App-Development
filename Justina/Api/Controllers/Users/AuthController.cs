using Application.Dtos.Users;
using Application.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Público - Autenticación")]  
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (result.StatusCode == 201)
            {
                return Created(
                    $"api/Users/{result.Data}",
                    new
                    {
                        Message = result.Message,
                        UserId = result.Data
                    }
                );
            }

            return BadRequest(result);
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result.StatusCode == 200)
            {
                return Ok(new
                {
                    Token = result.Data?.Token,
                    Message = result.Message,
                    Roles = result.Data?.Roles
                });
            }

            return Unauthorized(result);
        }

        //POST: api/Auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            string? userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new { Error = "No se pudo identificar al usuario desde el Token" });

            var result = await _authService.ChangePasswordAsync(dto, userEmail);

            if (result.StatusCode == 200)
                return Ok(result);

            return BadRequest(result);
        }
    }
}