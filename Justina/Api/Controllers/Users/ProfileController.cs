using Application.Dtos.Users;
using Application.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("Usuario - Perfil")]  // 👈 AGREGAR PARA SWAGGER
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return 0;

            return int.Parse(userIdClaim);
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { Error = "Usuario no identificado" });

            var result = await _userService.GetProfileAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { Error = "Usuario no identificado" });

            var result = await _userService.UpdateProfileAsync(userId, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        // ✅ NUEVO: Solicitar eliminación de cuenta
        [HttpPost("request-deletion")]
        public async Task<IActionResult> RequestDeletion()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { Error = "Usuario no identificado" });

            var result = await _userService.RequestAccountDeletionAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        // ✅ NUEVO: Cancelar solicitud de eliminación
        [HttpPost("cancel-deletion")]
        public async Task<IActionResult> CancelDeletion()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { Error = "Usuario no identificado" });

            var result = await _userService.CancelDeletionRequestAsync(userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}