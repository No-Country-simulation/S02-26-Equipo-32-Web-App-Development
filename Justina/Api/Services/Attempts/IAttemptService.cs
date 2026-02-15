using Api.Models.Attempts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Services.Attempts
{
    // Servicio de aplicación que orquesta la lógica de negocio relacionada con los intentos
    // de los juegos. Aquí traducimos DTOs a entidades de dominio y calculamos métricas.
    public interface IAttemptService
    {
        // Registra un nuevo intento a partir de los datos enviados por el frontend y devuelve
        // una representación de lectura para confirmación.
        Task<AttemptDto> RegisterAttemptAsync(CreateAttemptRequest request);

        // Recupera todos los intentos de un test concreto para poder analizarlos desde UI.
        Task<IReadOnlyList<AttemptDto>> GetAttemptsByTestAsync(int testId);

        // Calcula estadísticas agregadas a partir de todos los intentos de un test dado.
        Task<AttemptSummaryDto> GetSummaryByTestAsync(int testId);
    }
}

