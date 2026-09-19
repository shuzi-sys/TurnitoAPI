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
    }
}
