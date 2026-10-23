using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Models;

namespace TurnitoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
        }

        public DbSet<User> Usuarios { get; set; }
        public DbSet<Company> Empresas { get; set; }
        public DbSet<Appointment> Turnos { get; set; }
        public DbSet<Service> Servicios { get; set; }
        public DbSet<Provider> Prestadores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Service>(e =>
            {
                e.ToTable("Servicios", t =>
                    t.HasCheckConstraint("CK_Servicios_Duracion", "\"DurationMinutes\" BETWEEN 1 AND 1440"));
                e.HasIndex(s => new { s.CompanyID, s.Name }).IsUnique();   // opcional
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Usuarios", t =>
                    t.HasCheckConstraint("CK_Usuarios_Cuotas",
                        "\"AvailableCompanies\" >= 0 AND \"AvailableProviders\" >= 0 " +
                        "AND \"AvailableServices\" >= 0 AND \"AvailableAppointments\" >= 0"));
            });
        }
    }
}
