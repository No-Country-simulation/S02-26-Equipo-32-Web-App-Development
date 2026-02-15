using Api.Models.Attempts;
using Domain.Models;
using Infraestruture.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.Attempts
{
    // Implementación del servicio de intentos que usa el repositorio para acceder a la base de datos.
    // La responsabilidad principal de esta clase es adaptar los modelos de entrada/salida del API
    // (DTOs) a las entidades de dominio y aplicar la lógica de agregación de métricas.
    public class AttemptService : IAttemptService
    {
        private readonly IAttemptRepository _attemptRepository;

        // Inyectamos el repositorio para mantener el servicio enfocado en la lógica de negocio
        // y no en detalles de persistencia (consultas EF, conexiones, etc.).
        public AttemptService(IAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }

        // Crea un nuevo Attempt a partir del DTO de entrada, completando campos que deben
        // definirse en backend (como la fecha en caso de que no venga informada) y delega
        // la persistencia en el repositorio.
        public async Task<AttemptDto> RegisterAttemptAsync(CreateAttemptRequest request)
        {
            var attempt = new Attempt
            {
                UserId = request.UserId,
                TestId = request.TestId,
                Duration = request.Duration,
                Date = request.Date ?? DateTime.UtcNow,
                ErrorCount = request.ErrorCount,
                TrajectoryScore = request.TrajectoryScore,
                PrecisionScore = request.PrecisionScore,
                SmoothnessScore = request.SmoothnessScore,
                CompletionStatus = request.CompletionStatus
            };

            var saved = await _attemptRepository.AddAsync(attempt);

            return MapToDto(saved);
        }

        // Obtiene todos los intentos de un test y los mapea a DTOs para evitar que el cliente
        // tenga que conocer la estructura completa de las entidades de dominio.
        public async Task<IReadOnlyList<AttemptDto>> GetAttemptsByTestAsync(int testId)
        {
            var attempts = await _attemptRepository.GetByTestAsync(testId);
            return attempts.Select(MapToDto).ToList();
        }

        // A partir de los intentos de un test, calcula agregados básicos que el frontend
        // puede utilizar para mostrar estadísticas de rendimiento (intentos, promedios, mejores valores).
        public async Task<AttemptSummaryDto> GetSummaryByTestAsync(int testId)
        {
            var attempts = await _attemptRepository.GetByTestAsync(testId);

            if (attempts.Count == 0)
            {
                return new AttemptSummaryDto
                {
                    Attempts = 0,
                    AverageDuration = 0,
                    AverageErrorCount = 0,
                    BestPrecisionScore = null,
                    BestTrajectoryScore = null,
                    BestSmoothnessScore = null
                };
            }

            return new AttemptSummaryDto
            {
                Attempts = attempts.Count,
                AverageDuration = attempts.Average(a => a.Duration),
                AverageErrorCount = attempts.Average(a => a.ErrorCount),
                BestPrecisionScore = attempts.Max(a => a.PrecisionScore),
                BestTrajectoryScore = attempts.Max(a => a.TrajectoryScore),
                BestSmoothnessScore = attempts.Max(a => a.SmoothnessScore)
            };
        }

        // Método auxiliar privado para centralizar el mapeo de la entidad Attempt al DTO
        // y evitar duplicar esta lógica en varios métodos.
        private static AttemptDto MapToDto(Attempt attempt)
        {
            return new AttemptDto
            {
                Id = attempt.Id,
                UserId = attempt.UserId,
                TestId = attempt.TestId,
                Duration = attempt.Duration,
                Date = attempt.Date,
                ErrorCount = attempt.ErrorCount,
                TrajectoryScore = attempt.TrajectoryScore,
                PrecisionScore = attempt.PrecisionScore,
                SmoothnessScore = attempt.SmoothnessScore,
                CompletionStatus = attempt.CompletionStatus
            };
        }
    }
}

