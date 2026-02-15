using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infraestruture.Repository
{
    // Definimos una interfaz de repositorio específica para Attempts para aislar el acceso a datos
    // y evitar que las capas superiores conozcan los detalles de Entity Framework o del DbContext.
    public interface IAttemptRepository
    {
        // Método para registrar un nuevo intento en base de datos; devuelve la entidad persistida
        // para que capas superiores puedan conocer el Id generado y demás datos finales.
        Task<Attempt> AddAsync(Attempt attempt);

        // Método para recuperar todos los intentos de un test concreto; esto permitirá al servicio
        // calcular métricas como cantidad de intentos, tiempos promedio, etc.
        Task<List<Attempt>> GetByTestAsync(int testId);
    }
}

