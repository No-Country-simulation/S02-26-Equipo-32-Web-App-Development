using Api.Controllers.UsersRoles;
using Application.Dtos.UserRole;
using Application.Interface.Service.UsersRoles;
using Application.Service.UsersRoles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers.Reporte
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReposteController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<ReposteController> _logger;
        public ReposteController(IUserRoleService userRoleService,ILogger<ReposteController> logger)
        {
            _userRoleService = userRoleService ?? throw new ArgumentNullException(nameof(userRoleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// Obtiene todos los usuarios con un rol específico por nombre
        /// </summary>
        /// <param name="roleName">Nombre del rol (ej: "Cirujano")</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de usuarios con el rol especificado</returns>
        /// <response code="200">Lista de usuarios obtenida exitosamente</response>
        /// <response code="400">Nombre de rol inválido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("role/name/{roleName}")]
        [ProducesResponseType(typeof(IEnumerable<UserRoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRoleName(string roleName, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud GET recibida para obtener usuarios con rol de nombre: {RoleName}", roleName);

            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("Nombre de rol vacío o nulo proporcionado");
                return BadRequest(new { error = "El nombre del rol no puede estar vacío" });
            }

            var result = await _userRoleService.GetByRoleNameAsync(roleName, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al obtener usuarios con rol '{RoleName}': {Error}", roleName, result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
            }

            return Ok(result.Value);
        }

    }
}
