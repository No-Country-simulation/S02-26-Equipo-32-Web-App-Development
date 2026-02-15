using System;

namespace Api.Models.Attempts
{
    // DTO de salida para devolver información de un intento a los clientes.
    // Usamos un modelo plano pensado para lectura, evitando exponer navegaciones complejas.
    public class AttemptDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int Duration { get; set; }
        public DateTime Date { get; set; }
        public int ErrorCount { get; set; }
        public decimal? TrajectoryScore { get; set; }
        public decimal? PrecisionScore { get; set; }
        public decimal? SmoothnessScore { get; set; }
        public string CompletionStatus { get; set; } = null!;
    }
}

