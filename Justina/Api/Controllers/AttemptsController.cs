using Api.Models.Attempts;
using Api.Services.Attempts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    // Controlador API encargado de exponer endpoints HTTP relacionados con los intentos de juego.
    // Se apoya en el servicio de intentos para mantener la lógica de negocio fuera de la capa web.
    [ApiController]
    [Route("api/[controller]")]
    public class AttemptsController : ControllerBase
    {
        private readonly IAttemptService _attemptService;

        // Inyectamos el servicio para seguir el patrón de dependencia hacia abstracciones
        // y permitir pruebas unitarias del controlador sin tocar la base de datos real.
        public AttemptsController(IAttemptService attemptService)
        {
            _attemptService = attemptService;
        }

        // Endpoint POST para registrar un nuevo intento. Recibe el DTO de entrada,
        // delega el proceso al servicio y devuelve el DTO de salida con Created (201).
        [HttpPost]
        public async Task<ActionResult<AttemptDto>> Create([FromBody] CreateAttemptRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _attemptService.RegisterAttemptAsync(request);
            return CreatedAtAction(nameof(GetByTest), new { testId = result.TestId }, result);
        }

        // Endpoint GET para obtener todos los intentos de un test concreto; esto permite
        // al frontend listar la historia de intentos de un juego.
        [HttpGet("test/{testId:int}")]
        public async Task<ActionResult<IReadOnlyList<AttemptDto>>> GetByTest(int testId)
        {
            var attempts = await _attemptService.GetAttemptsByTestAsync(testId);
            return Ok(attempts);
        }

        // Endpoint GET que devuelve estadísticas agregadas para un test. Es útil para dashboards
        // y pantallas de resultados sin tener que recalcular promedios en el cliente.
        [HttpGet("test/{testId:int}/summary")]
        public async Task<ActionResult<AttemptSummaryDto>> GetSummary(int testId)
        {
            var summary = await _attemptService.GetSummaryByTestAsync(testId);
            return Ok(summary);
        }
    }
}

