using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Service;
using TurnitoAPI.Services.Interfaces;
namespace TurnitoAPI.Services
{
    public class serviceService : IserviceService
    {
        private readonly AppDbContext _context;
        public serviceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Get_Service_Dto> CreateServiceAsync(int userId, Create_Service_Dto dto) {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (user.AvailableServices <= 0)
            {
                throw new Exception("User has no available services left");
            }
            var company = await _context.Empresas.FindAsync(dto.CompanyID);
            if (company == null || company.UserId != userId) {
                throw new Exception("Not an owner of such company");
            }
            var service = new Service
            {
                Name = dto.Name,
                DurationMinutes = dto.DurationMinutes,
                CompanyID = dto.CompanyID
            };
            _context.Servicios.Add(service);
            user.AvailableServices--;
            await _context.SaveChangesAsync();
            return new Get_Service_Dto
            {
                Id = service.Id,
                Name = service.Name,
                DurationMinutes = service.DurationMinutes,
                CompanyID = service.CompanyID
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
            var service = await _context.Servicios
                .FirstOrDefaultAsync(s=> s.Id == serviceId
                                    && s.Company.UserId == userId);
            if (service == null)
            {
                return false;
            }
            _context.Servicios.Remove(service);
            var user = await _context.Usuarios.FindAsync(userId);
            user!.AvailableServices++; // <- i bypass the boolean because it's guaranteed to exist by service query
            await _context.SaveChangesAsync();
            return true;
        }
    }
}