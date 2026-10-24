using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Service;
using TurnitoAPI.Services.Interfaces;
using TurnitoAPI.Exceptions;
namespace TurnitoAPI.Services
{
    public class serviceService : IserviceService
    {
        private readonly AppDbContext _context;
        public serviceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Get_Service_Dto> CreateServiceAsync(int userId, Create_Service_Dto dto)
        {
            var companyName = await _context.Empresas
                .Where(c => c.Id == dto.CompanyID && c.UserId == userId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
            if (companyName == null)
            {
                throw new ForbiddenException("Not an owner of such company");
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            var updated = await _context.Usuarios
                .Where(u => u.Id == userId && u.AvailableServices > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.AvailableServices, u => u.AvailableServices - 1));
            if (updated == 0)
            {
                throw new BusinessRuleException("User has no available services left");
            }

            var service = new Service
            {
                Name = dto.Name.Trim(),
                DurationMinutes = dto.DurationMinutes,
                CompanyID = dto.CompanyID
            };
            _context.Servicios.Add(service);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new Get_Service_Dto
            {
                Id = service.Id,
                Name = service.Name,
                DurationMinutes = service.DurationMinutes,
                CompanyID = service.CompanyID,
                CompanyName = companyName
            };
        }

        public async Task<IEnumerable<Get_Service_Dto>> GetServicesByCompanyIdAsync(int userId, int id)
        {
            var ownsCompany = await _context.Empresas.AnyAsync(c => c.Id == id && c.UserId == userId);
            if (!ownsCompany)
            {
                return null; 
            }
            var services = await _context.Servicios
        .AsNoTracking()
        .Where(s => s.CompanyID == id)
        .Select(s => new Get_Service_Dto
        {
            Id = s.Id,
            Name = s.Name,
            DurationMinutes = s.DurationMinutes,
            CompanyID = s.CompanyID,
            CompanyName = s.Company.Name
        })
        .ToListAsync();
            return services;
        }

        public async Task<bool> DeleteServiceById(int userId, int serviceId)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            var deleted = await _context.Servicios
                .Where(s => s.Id == serviceId && s.Company.UserId == userId)
                .ExecuteDeleteAsync();
            if (deleted == 0) return false;

            await _context.Usuarios
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.AvailableServices, u => u.AvailableServices + 1));

            await tx.CommitAsync();
            return true;
        }
    }
}