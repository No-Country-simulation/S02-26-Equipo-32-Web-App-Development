using Domain.Models;
using Infraestruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infraestruture.Repository
{
    // Implementación concreta del repositorio de Attempts basada en Entity Framework Core.
    // Esta clase se encarga exclusivamente de hablar con la base de datos y no contiene lógica
    // de negocio; así mantenemos una separación clara de responsabilidades.
    public class AttemptRepository : IAttemptRepository
    {
        private readonly JustinaDbContext _context;

        // Inyectamos el DbContext para aprovechar el contenedor de dependencias configurado en Program.cs
        // y poder reutilizar la misma conexión por request HTTP.
        public AttemptRepository(JustinaDbContext context)
        {
            _context = context;
        }

        // Inserta un nuevo Attempt en la base de datos y garantiza que los cambios se persisten
        // mediante SaveChangesAsync. Devolvemos la entidad para que capas superiores puedan usar
        // el Id y cualquier valor calculado por EF.
        public async Task<Attempt> AddAsync(Attempt attempt)
        {
            _context.Attempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        // Recupera todos los intentos asociados a un Test concreto para poder calcular estadísticas
        // de desempeño por juego (test) sin exponer directamente consultas LINQ fuera del repositorio.
        public async Task<List<Attempt>> GetByTestAsync(int testId)
        {
            return await _context.Attempts
                .AsNoTracking()
                .Where(a => a.TestId == testId)
                .ToListAsync();
        }
    }
}

