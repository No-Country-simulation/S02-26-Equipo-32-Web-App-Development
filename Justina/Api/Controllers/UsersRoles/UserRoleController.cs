using Application.Dtos.UserRole;
using Application.Interface.Service.UsersRoles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.UsersRoles
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<UserRoleController> _logger;

        public UserRoleController(
            IUserRoleService userRoleService,
            ILogger<UserRoleController> logger)
        {
            _userRoleService = userRoleService ?? throw new ArgumentNullException(nameof(userRoleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los UserRoles
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de UserRoles</returns>
        /// <response code="200">Lista de UserRoles obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserRoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud GET recibida para obtener todos los UserRoles");

            var result = await _userRoleService.GetAllAsync(cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al obtener UserRoles: {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Obtiene un UserRole por su Id
        /// </summary>
        /// <param name="id">Id del UserRole</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>UserRole solicitado</returns>
        /// <response code="200">UserRole encontrado</response>
        /// <response code="400">Id inválido</response>
        /// <response code="404">UserRole no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserRoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud GET recibida para obtener UserRole con Id: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Id inválido proporcionado: {Id}", id);
                return BadRequest(new { error = "El Id debe ser mayor a 0" });
            }

            var result = await _userRoleService.GetByIdAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("UserRole con Id {Id} no encontrado", id);
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Obtiene todos los roles de un usuario específico
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de roles del usuario</returns>
        /// <response code="200">Lista de roles obtenida exitosamente</response>
        /// <response code="400">UserId inválido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<UserRoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud GET recibida para obtener roles del usuario: {UserId}", userId);

            if (userId <= 0)
            {
                _logger.LogWarning("UserId inválido proporcionado: {UserId}", userId);
                return BadRequest(new { error = "El UserId debe ser mayor a 0" });
            }

            var result = await _userRoleService.GetByUserIdAsync(userId, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al obtener roles del usuario {UserId}: {Error}", userId, result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Obtiene todos los usuarios con un rol específico
        /// </summary>
        /// <param name="roleId">Id del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de usuarios con el rol</returns>
        /// <response code="200">Lista de usuarios obtenida exitosamente</response>
        /// <response code="400">RoleId inválido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("role/{roleId:int}")]
        [ProducesResponseType(typeof(IEnumerable<UserRoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRoleId(int roleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud GET recibida para obtener usuarios con rol: {RoleId}", roleId);

            if (roleId <= 0)
            {
                _logger.LogWarning("RoleId inválido proporcionado: {RoleId}", roleId);
                return BadRequest(new { error = "El RoleId debe ser mayor a 0" });
            }

            var result = await _userRoleService.GetByRoleIdAsync(roleId, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al obtener usuarios con rol {RoleId}: {Error}", roleId, result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Crea un nuevo UserRole
        /// </summary>
        /// <param name="createDto">Datos del UserRole a crear</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>UserRole creado</returns>
        /// <response code="201">UserRole creado exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        /// <response code="409">La asignación de rol ya existe</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserRoleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateUserRoleDto createDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud POST recibida para crear UserRole");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos inválidos en la solicitud de creación");
                return BadRequest(ModelState);
            }

            var result = await _userRoleService.CreateAsync(createDto, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al crear UserRole: {Error}", result.Error);

                if (result.Error.Contains("Ya existe"))
                {
                    return Conflict(new { error = result.Error });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
            }

            _logger.LogInformation("UserRole creado exitosamente con Id: {Id}", result.Value.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.Id },
                result.Value);
        }

        /// <summary>
        /// Actualiza un UserRole existente
        /// </summary>
        /// <param name="id">Id del UserRole a actualizar</param>
        /// <param name="updateDto">Datos actualizados del UserRole</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>UserRole actualizado</returns>
        /// <response code="200">UserRole actualizado exitosamente</response>
        /// <response code="400">Datos inválidos o Id no coincide</response>
        /// <response code="404">UserRole no encontrado</response>
        /// <response code="409">La nueva asignación de rol ya existe</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserRoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRoleDto updateDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud PUT recibida para actualizar UserRole con Id: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Id inválido proporcionado: {Id}", id);
                return BadRequest(new { error = "El Id debe ser mayor a 0" });
            }

            if (id != updateDto.Id)
            {
                _logger.LogWarning("El Id de la URL {UrlId} no coincide con el Id del DTO {DtoId}", id, updateDto.Id);
                return BadRequest(new { error = "El Id de la URL no coincide con el Id del objeto" });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos inválidos en la solicitud de actualización");
                return BadRequest(ModelState);
            }

            var result = await _userRoleService.UpdateAsync(updateDto, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al actualizar UserRole con Id {Id}: {Error}", id, result.Error);

                if (result.Error.Contains("no encontrado"))
                {
                    return NotFound(new { error = result.Error });
                }

                if (result.Error.Contains("Ya existe"))
                {
                    return Conflict(new { error = result.Error });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
            }

            _logger.LogInformation("UserRole con Id {Id} actualizado exitosamente", id);

            return Ok(result.Value);
        }

        /// <summary>
        /// Elimina un UserRole por su Id
        /// </summary>
        /// <param name="id">Id del UserRole a eliminar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">UserRole eliminado exitosamente</response>
        /// <response code="400">Id inválido</response>
        /// <response code="404">UserRole no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud DELETE recibida para UserRole con Id: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Id inválido proporcionado: {Id}", id);
                return BadRequest(new { error = "El Id debe ser mayor a 0" });
            }

            var result = await _userRoleService.DeleteAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al eliminar UserRole con Id {Id}: {Error}", id, result.Error);

                if (result.Error.Contains("no encontrado"))
                {
                    return NotFound(new { error = result.Error });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
            }

            _logger.LogInformation("UserRole con Id {Id} eliminado exitosamente", id);

            return NoContent();
        }
    }
}
