using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Provider;
using TurnitoAPI.Dtos.Service;
using Microsoft.EntityFrameworkCore;
namespace TurnitoAPI.Services
{
    public class providerService
    {
        private readonly AppDbContext _context;
        public providerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Get_Provider_Dto> CreateProvider(Create_Provider_Dto dto)
        {
            var serviceIds = dto.ServicesID.Distinct().ToList();
            var services = await _context.Servicios.Where(s => serviceIds.Contains(s.id) && s.CompanyID == dto.CompanyId)
                .ToListAsync();

            if (services.Count != serviceIds.Count)
            {
                throw new Exception("One or more services do not belong to the specified company.");
            }

            var newProvider = new Provider
            {
                Name = dto.Name,
                CompanyId = dto.CompanyId,
                PhoneNumber = dto.PhoneNumber,
                Services = services
            };
            _context.Prestadores.Add(newProvider);
            await _context.SaveChangesAsync();
            return await _context.Prestadores.Where(p => p.Id == newProvider.Id)
                .Where(p => p.Id == newProvider.Id)
                .Select(p => new Get_Provider_Dto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company.name,
                    Services = p.Services.Select(s => new Get_Service_Summary_Dto
                    {
                        Id = s.id,
                        Name = s.Name,
                        Duration = s.DurationMinutes
                    }).ToList()
                }).FirstAsync();
        }

        public async Task<Get_Provider_Dto> UpdateProvider(int id, Update_Provider_Dto dto)
        {
            var provider = await _context.Prestadores.Include(p => p.Services).FirstOrDefaultAsync(p => p.Id == id);
            if (provider == null)
            {
                throw new Exception("Provider not found.");
            }
            var serviceIds = dto.ServicesID.Distinct().ToList();
            if (serviceIds.Count > 0) {
            var services = await _context.Servicios.Where(s => serviceIds.Contains(s.id) && s.CompanyID == provider.CompanyId)
                .ToListAsync();
            if (services.Count != serviceIds.Count)
            {
                throw new Exception("One or more services do not belong to the specified company.");
            }
                provider.Services = services;
            }
            if (dto.Name != null)
            {
                provider.Name = dto.Name;
            }
            if (dto.PhoneNumber != null)
            { 
            provider.PhoneNumber = dto.PhoneNumber;
            }

            await _context.SaveChangesAsync();
            return await _context.Prestadores.Where(p => p.Id == provider.Id)
                .Select(p => new Get_Provider_Dto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company.name,
                    Services = p.Services.Select(s => new Get_Service_Summary_Dto
                    {
                        Id = s.id,
                        Name = s.Name,
                        Duration = s.DurationMinutes
                    }).ToList()
                }).FirstAsync();
        }

        public async Task<List<Get_Provider_Dto>> GetAllProvidersForCompany(int companyId)
        {
            return await _context.Prestadores
                .Where(p => p.CompanyId == companyId)
                .Select(p => new Get_Provider_Dto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company.name,
                    Services = p.Services.Select(s => new Get_Service_Summary_Dto
                    {
                        Id = s.id,
                        Name = s.Name,
                        Duration = s.DurationMinutes
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<Get_Provider_Dto?> GetProviderById(int id)
        {
            return await _context.Prestadores
                .Where(p => p.Id == id)
                .Select(p => new Get_Provider_Dto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company.name,
                    Services = p.Services.Select(s => new Get_Service_Summary_Dto
                    {
                        Id = s.id,
                        Name = s.Name,
                        Duration = s.DurationMinutes
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteProvider(int id)
        {
            var provider = await _context.Prestadores.FindAsync(id);
            if (provider == null)
            {
                return false;
            }
            _context.Prestadores.Remove(provider);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    }
