using System;

namespace Api.Models.Attempts
{
    // DTO de entrada para registrar un intento desde el frontend.
    // Separar este modelo de la entidad de dominio evita exponer toda la estructura interna
    // y nos permite controlar qué datos puede enviar el cliente.
    public class CreateAttemptRequest
    {
        // Identificador del usuario que realiza el intento; se deja como entero genérico
        // para acoplarse al modelo de Users ya definido en el dominio.
        public int UserId { get; set; }

        // Identificador del test (juego) asociado; esto permite agrupar intentos por tipo de prueba.
        public int TestId { get; set; }

        // Duración del intento en milisegundos (o la unidad que mande el frontend),
        // se guarda como entero para simplificar el almacenamiento y cálculos posteriores.
        public int Duration { get; set; }

        // Cantidad de errores cometidos durante el intento, útil para métricas de precisión.
        public int ErrorCount { get; set; }

        // Puntuación relacionada con seguimiento de trayectoria; opcional para no forzar
        // que todos los juegos calculen exactamente este valor.
        public decimal? TrajectoryScore { get; set; }

        // Puntuación de precisión general del intento; puede mapearse a la "perfection"
        // que calcula el frontend.
        public decimal? PrecisionScore { get; set; }

        // Puntuación de suavidad de movimiento; también opcional para que distintos juegos
        // puedan enviar métricas diferentes.
        public decimal? SmoothnessScore { get; set; }

        // Estado de finalización del intento (Completado, Cancelado, Fallido, etc.);
        // se modela como string para permitir flexibilidad inicial antes de fijar un enum.
        public string CompletionStatus { get; set; } = null!;

        // Fecha/hora en la que ocurrió el intento; si el frontend no la envía se puede
        // completar en el servicio con DateTime.UtcNow para mantener coherencia temporal.
        public DateTime? Date { get; set; }
    }
}

