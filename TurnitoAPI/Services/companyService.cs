using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Dtos.Company;
using TurnitoAPI.Exceptions;
using TurnitoAPI.Models;
using TurnitoAPI.Services.Interfaces;
namespace TurnitoAPI.Services
{
    public class companyService : IcompanyService
    {
        private readonly AppDbContext _context;
        public companyService(AppDbContext context) { this._context = context; }

        public async Task<List<Get_Company_Dto>> GetAllCompanies(int userId)
        {

            var isAdmin = await _context.Usuarios.AnyAsync(u => u.Id == userId && u.isAdmin);
            if (!isAdmin)
                throw new ForbiddenException("You dont have permission for this.");

            return await _context.Empresas
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new Get_Company_Dto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }

        public async Task<Get_Company_Dto> GetCompanyById(int userId, int id)
        {
            return await _context.Empresas
                .AsNoTracking()
                .Where(c => c.Id == id && c.UserId == userId)
                .Select(c => new Get_Company_Dto { Id = c.Id, Name = c.Name })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Company not found.");
        }

        public async Task<ICollection<Get_Company_Dto>> GetUserCompanies(int userId)
        {
            return await _context.Empresas.Where(c => c.UserId == userId)
                .Select(c => new Get_Company_Dto { Id = c.Id, Name = c.Name })
                .ToListAsync() ?? throw new NotFoundException("No companies found");
        }

        public async Task<Get_Company_Summary_Dto> GetCompanySummaryById(int userId, int id)
        {
            return await _context.Empresas
            .AsNoTracking()
            .Where(c => c.Id == id && c.UserId == userId)
            .Select(c => new Get_Company_Summary_Dto { Id = c.Id, Name = c.Name, AppointmentsCount = c.Appointments.Count, 
            ServicesCount = c.Services.Count, ProvidersCount = c.Appointments.Count})
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Company not found.");
        }

        public async Task<ICollection<Get_Company_Summary_Dto>> GetUserCompaniesSummary(int userId)
        {
            return await _context.Empresas.Where(c => c.UserId == userId)
                .Select(c => new Get_Company_Summary_Dto {
                    Id = c.Id,
                    Name = c.Name,
                    AppointmentsCount = c.Appointments.Count,
                    ServicesCount = c.Services.Count,
                    ProvidersCount = c.Appointments.Count
                })
                .ToListAsync() ?? throw new NotFoundException("No companies found");
        }

        // metele los dtos al dto del get-company y devolvelo cuando termines lo demas.
        public async Task<Get_Company_Dto> CreateCompany(int userId, Create_Company_Dto companyDto)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            // Chequeo + descuento atómico (si el usuario no existe, también da 0)
            var updated = await _context.Usuarios
                .Where(u => u.Id == userId && u.AvailableCompanies > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    u => u.AvailableCompanies, u => u.AvailableCompanies - 1));
            if (updated == 0)
                throw new BusinessRuleException("No companies available.");

            var company = new Company
            {
                UserId = userId,
                Name = companyDto.name.Trim()
            };
            _context.Empresas.Add(company);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new Get_Company_Dto { Id = company.Id, Name = company.Name };
        }

        public async Task<Get_Company_Dto> UpdateCompanyName(int userId, int companyId, Update_Company_Dto  companyDto)
        {
            var company = await _context.Empresas
                .FirstOrDefaultAsync(c => c.Id == companyId && c.UserId == userId)
                ?? throw new NotFoundException("Company not found.");

            company.Name = companyDto.Name.Trim();
            await _context.SaveChangesAsync();

            return new Get_Company_Dto { Id = company.Id, Name = company.Name };
        }

        public async Task DeleteCompany(int userId, int id)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            // Cuento lo que se va a borrar en cascada para devolver los cupos
            var counts = await _context.Empresas
                .AsNoTracking()
                .Where(c => c.Id == id && c.UserId == userId)
                .Select(c => new
                {
                    Services = c.Services.Count,
                    Providers = c.Providers.Count,
                    Appointments = c.Appointments.Count
                })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Empresa no encontrada.");

            // La cascada la hace la base (los FK están en Cascade)
            var deleted = await _context.Empresas
                .Where(c => c.Id == id && c.UserId == userId)
                .ExecuteDeleteAsync();

            if (deleted == 0) { throw new NotFoundException("Empresa no encontrada."); }

            await _context.Usuarios
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AvailableCompanies, u => u.AvailableCompanies + 1)
                    .SetProperty(u => u.AvailableServices, u => u.AvailableServices + counts.Services)
                    .SetProperty(u => u.AvailableProviders, u => u.AvailableProviders + counts.Providers)
                    .SetProperty(u => u.AvailableAppointments, u => u.AvailableAppointments + counts.Appointments));

            await tx.CommitAsync();
        }

    }
}
