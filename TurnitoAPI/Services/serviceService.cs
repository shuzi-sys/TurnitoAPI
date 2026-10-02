using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Service;
namespace TurnitoAPI.Services
{
    public class serviceService
    {
        private readonly AppDbContext _context;
        public serviceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Get_Service_Dto> CreateServiceAsync(Create_Service_Dto dto) {
            var service = new Service
            {
                Name = dto.Name,
                DurationMinutes = dto.DurationMinutes,
                CompanyID = dto.CompanyID
            };
            _context.Servicios.Add(service);
            await _context.SaveChangesAsync();
            return new Get_Service_Dto
            {
                Id = service.Id,
                Name = service.Name,
                DurationMinutes = service.DurationMinutes,
                CompanyID = service.CompanyID
            };
        }

        public async Task<IEnumerable<Get_Service_Dto>> GetServicesByCompanyIdAsync(int id)
        {
            var services = await _context.Servicios.Where(s => s.CompanyID == id).ToListAsync();
            return services.Select(s => new Get_Service_Dto
            {
                Id = s.Id,
                Name = s.Name,
                DurationMinutes = s.DurationMinutes,
                CompanyID = s.CompanyID
            });
        }

        public async Task<bool> DeleteServiceById(int selfcompanyId, int serviceId)
        {
            var service = await _context.Servicios.FindAsync(serviceId);
            if (service == null)
            {
                return false;
            }
            if (service.CompanyID != selfcompanyId)
            {
                return false;
            }
            _context.Servicios.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}