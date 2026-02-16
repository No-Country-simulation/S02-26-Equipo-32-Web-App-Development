using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestruture.Persistence.Context
{
    public class JustinaDbContext : DbContext
    {
        // El constructor recibe las opciones (como la cadena de conexión) desde el Program.cs
        public JustinaDbContext(DbContextOptions<JustinaDbContext> options)
            : base(options)
        {
        }

        // Definición de las tablas (DbSets)
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Esta línea es la más importante: 
            // Busca automáticamente todas las clases de configuración (Fluent API) 
            // que creamos en la carpeta "Configurations" y las aplica.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JustinaDbContext).Assembly);
        }
    }
}

