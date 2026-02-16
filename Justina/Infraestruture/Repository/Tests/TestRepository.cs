using Application.Interface.Repository.Tests;
using Domain.Models;
using Infraestruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestruture.Repository.Tests
{
    public class TestRepository : ITestRepository
    {
        private readonly JustinaDbContext _context;

        public TestRepository(JustinaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Test?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Test>()
                .Include(t => t.Difficulty)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Test>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Test>()
                .Include(t => t.Difficulty)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Test> AddAsync(Test test, CancellationToken cancellationToken = default)
        {
            var entry = await _context.Set<Test>().AddAsync(test, cancellationToken);
            return entry.Entity;
        }

        public Test? Update(Test test)
        {
            var existingTest = _context.Set<Test>()
                .FirstOrDefault(t => t.Id == test.Id);

            if (existingTest == null)
                return null;

            _context.Entry(existingTest).CurrentValues.SetValues(test);
            return existingTest;
        }

        public bool Delete(int id)
        {
            var test = _context.Set<Test>()
                .FirstOrDefault(t => t.Id == id);

            if (test == null)
                return false;

            _context.Set<Test>().Remove(test);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Test>()
                .AnyAsync(t => t.Id == id, cancellationToken);
        }
    }
}
