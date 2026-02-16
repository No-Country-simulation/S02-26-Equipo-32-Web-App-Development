using Domain.Models;
using Infraestruture.Persistence.Configurations;
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
        public JustinaDbContext(DbContextOptions<JustinaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Tus configuraciones existentes...
            modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JustinaDbContext).Assembly);
        }
    }
}

