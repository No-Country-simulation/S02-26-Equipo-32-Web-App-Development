using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository.Tests
{
    public interface ITestRepository
    {
        Task<Test?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Test>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Test> AddAsync(Test test, CancellationToken cancellationToken = default);
        Test? Update(Test test);
        bool Delete(int id);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}
