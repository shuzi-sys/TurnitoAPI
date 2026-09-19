using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Models;

namespace TurnitoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
        }

        public DbSet<Company> Empresas { get; set; }
        public DbSet<Appointment> Turnos { get; set; }
        public DbSet<Service> Servicios { get; set; }
        public DbSet<Provider> Prestadores { get; set; }
    }
}
