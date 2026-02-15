namespace Api.Models.Attempts
{
    // DTO de agregados estadísticos para un test concreto; resume los intentos registrados
    // y permite al frontend mostrar métricas sin tener que recalcularlas en cliente.
    public class AttemptSummaryDto
    {
        // Número total de intentos realizados para el test.
        public int Attempts { get; set; }

        // Promedio de duración de los intentos.
        public double AverageDuration { get; set; }

        // Promedio de errores cometidos por intento.
        public double AverageErrorCount { get; set; }

        // Mejor puntuación de precisión registrada (máximo PrecisionScore).
        public decimal? BestPrecisionScore { get; set; }

        // Mejor puntuación de trayectoria registrada (máximo TrajectoryScore).
        public decimal? BestTrajectoryScore { get; set; }

        // Mejor puntuación de suavidad registrada (máximo SmoothnessScore).
        public decimal? BestSmoothnessScore { get; set; }
    }
}

