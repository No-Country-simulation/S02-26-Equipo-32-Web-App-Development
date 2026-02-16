using Application.Dtos.Auth;
using Application.Dtos.Common;
using Application.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Público - Recuperación")]
    public class PasswordResetController : ControllerBase
    {
        private readonly PasswordResetService _passwordResetService;

        public PasswordResetController(PasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _passwordResetService.ForgotPasswordAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _passwordResetService.ResetPasswordAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
