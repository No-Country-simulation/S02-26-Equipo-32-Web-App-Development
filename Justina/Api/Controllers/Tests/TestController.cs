using Application.Dtos.Tests;
using Application.Interface.Service.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Tests
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly ILogger<TestController> _logger;

        public TestController(ITestService testService, ILogger<TestController> logger)
        {
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los tests
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _testService.GetAllAsync(cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al obtener tests: {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }

            return Ok(result.Value);
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
        /// Crea un nuevo test
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TestResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] TestRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _testService.CreateAsync(request, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error al crear test: {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Actualiza un test existente
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

        /// <summary>
        /// Elimina un test
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _testService.DeleteAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error.Contains("no encontrado"))
                    return NotFound(result.Error);

                _logger.LogError("Error al eliminar test: {Error}", result.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }

            return NoContent();
        }
    }
}
    

