using Application.Interface.Repository;        // 👈 FALTABA
using Application.Interface.UnitOfWork;        // 👈 YA ESTÁ
using Infraestruture.Persistence.Context;     // 👈 YA ESTÁ
using Infraestruture.Repository;              // 👈 FALTABA
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infraestruture.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly JustinaDbContext _context;
        private IUserRepository _userRepository = null!;     
        private IRoleRepository _roleRepository = null!;      
        private IAttemptRepository _attemptRepository = null!; 

        public UnitOfWork(JustinaDbContext context)
        {
            _context = context;
        }

        // Repositorios (Lazy initialization)
        public IUserRepository Users =>
            _userRepository ??= new UserRepository(_context);

        public IRoleRepository Roles =>
            _roleRepository ??= new RoleRepository(_context);

        public IAttemptRepository Attempts =>
            _attemptRepository ??= new AttemptRepository(_context);

        // Métodos de transacción
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this); // 👈 AGREGAR ESTA LÍNEA
        }
    }
}