using Api.Controllers.Tests;
using Application.Dtos.Tests;
using Application.Interface.Service.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Pruebas
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebaController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly ILogger<PruebaController> _logger;

        public PruebaController(ITestService testService,ILogger<PruebaController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _testService = testService ?? throw new ArgumentNullException(nameof(testService)); 
        }
        /// <summary>
        /// Obtiene un test por ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _testService.GetByIdAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogWarning("Test con ID {TestId} no encontrado", id);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
        /// <summary>
        /// Actualiza un test existente. No sólo tiempos sino tambien otros atributos
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] TestUpdateDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest("El ID de la ruta no coincide con el ID del body.");

            var result = await _testService.UpdateAsync(request, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error.Contains("no encontrado"))
                    return NotFound(result.Error);

                _logger.LogError("Error al actualizar test: {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }

            return Ok(result.Value);
        }

    }
}
