using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    /// <summary>
    /// Interfaz genérica para operaciones básicas de repositorio
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        // 📌 CONSULTAS
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        // 📌 OPERACIONES
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        // 📌 GUARDADO
        Task<int> SaveChangesAsync();
    }
}
