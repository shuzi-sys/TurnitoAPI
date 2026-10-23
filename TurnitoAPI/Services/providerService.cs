using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TurnitoAPI.Data;
using TurnitoAPI.Dtos.Provider;
using TurnitoAPI.Dtos.Service;
using TurnitoAPI.Exceptions;
using TurnitoAPI.Models;
using TurnitoAPI.Services.Interfaces;
namespace TurnitoAPI.Services
{
    public class providerService : IproviderService
    {
        private readonly AppDbContext _context;
        public providerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Get_Provider_Dto> CreateProvider(int userId, Create_Provider_Dto dto)
        {
            var ownsCompany = await _context.Empresas
                .AnyAsync(c => c.Id == dto.CompanyId && c.UserId == userId);
            if (!ownsCompany)
            {
                throw new Exception("User does not belong to the specified company.");
            }

            var serviceIds = (dto.ServicesID ?? new List<int>()).Distinct().ToList();
            var services = await _context.Servicios
                .Where(s => serviceIds.Contains(s.Id) && s.CompanyID == dto.CompanyId)
                .ToListAsync();
            if (services.Count != serviceIds.Count)
            {
                throw new Exception("One or more services do not belong to the specified company.");
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            // evita la race de cuota
            var updated = await _context.Usuarios
                .Where(u => u.Id == userId && u.AvailableProviders > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.AvailableProviders, u => u.AvailableProviders - 1));
            if (updated == 0)
            {
                throw new Exception("User has no available providers left.");
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
            await tx.CommitAsync();

            return new Get_Provider_Dto
            {
                Id = newProvider.Id,
                Name = newProvider.Name,
                CompanyId = newProvider.CompanyId,
                PhoneNumber = newProvider.PhoneNumber,
                Services = services.Select(s => new Get_Service_Dto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DurationMinutes = s.DurationMinutes,
                    CompanyID = s.CompanyID
                }).ToList()
            };
        }

        public async Task<Get_Provider_Dto> UpdateProvider(int userId, int id, Update_Provider_Dto dto)
        {
            var provider = await _context.Prestadores
                .Include(p => p.Services)
                .FirstOrDefaultAsync(p => p.Id == id && p.Company.UserId == userId);
            if (provider == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name)) provider.Name = dto.Name.Trim();
            if (dto.PhoneNumber != null) provider.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
            return new Get_Provider_Dto
            {
                Id = provider.Id,
                Name = provider.Name,
                CompanyId = provider.CompanyId,
                PhoneNumber = provider.PhoneNumber,
                Services = provider.Services.Select(s => new Get_Service_Dto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DurationMinutes = s.DurationMinutes,
                    CompanyID = s.CompanyID
                }).ToList()
            };
        }

        public async Task<Get_Provider_Dto> AddServiceToProvider(int userId, int providerId, List<int> serviceIds)
        {
            var ids = serviceIds.Distinct().ToList();
            if (ids.Count == 0 || ids.Count > 100) return null;

            var provider = await _context.Prestadores
                .Include(p => p.Services)
                .FirstOrDefaultAsync(p => p.Id == providerId && p.Company.UserId == userId);

            if (provider == null) return null;

            var services = await _context.Servicios
                .Where(s => ids.Contains(s.Id) && s.CompanyID == provider.CompanyId)
                .ToListAsync();

            if (services.Count != ids.Count) return null;

            var current = provider.Services.Select(s => s.Id).ToHashSet();
            foreach (var service in services.Where(s => !current.Contains(s.Id)))
                provider.Services.Add(service);

            await _context.SaveChangesAsync();

            return new Get_Provider_Dto
            {
                Id = provider.Id,
                Name = provider.Name,
                CompanyId = provider.CompanyId,
                PhoneNumber = provider.PhoneNumber,
                Services = provider.Services.Select(s => new Get_Service_Dto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DurationMinutes = s.DurationMinutes,
                    CompanyID = s.CompanyID
                }).ToList()
            };
        }

        public async Task<Get_Provider_Dto> RemoveServiceFromProvider(int userId, int providerId, List<int> serviceIds)
        {
            var ids = serviceIds.Distinct().ToList();
            if (ids.Count == 0 || ids.Count > 100) return null;
            var provider = await _context.Prestadores
                .Include(p => p.Services)
                .FirstOrDefaultAsync(p => p.Id == providerId && p.Company.UserId == userId);
            if (provider == null) return null;
            var servicesToRemove = provider.Services.Where(s => ids.Contains(s.Id)).ToList();
            foreach (var service in servicesToRemove)
                provider.Services.Remove(service);
            await _context.SaveChangesAsync();
            return new Get_Provider_Dto
            {
                Id = provider.Id,
                Name = provider.Name,
                CompanyId = provider.CompanyId,
                PhoneNumber = provider.PhoneNumber,
                Services = provider.Services.Select(s => new Get_Service_Dto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DurationMinutes = s.DurationMinutes,
                    CompanyID = s.CompanyID
                }).ToList()
            };
        }

        public async Task<List<Get_Provider_Dto>> GetAllProvidersForCompany(int userId, int companyId)
        {
            var user = await _context.Usuarios.Include(u => u.Company).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Company == null || !user.Company.Any(c => c.Id == companyId))
            {
                throw new Exception("User does not belong to the specified company.");
            }

            return await _context.Prestadores
                .Where(p => p.CompanyId == companyId)
                .Select(p => new Get_Provider_Dto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    PhoneNumber = p.PhoneNumber,
                    Services = p.Services.Select(s => new Get_Service_Dto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        DurationMinutes = s.DurationMinutes,
                        CompanyID = s.CompanyID
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<Get_Provider_Dto?> GetProviderById(int userId, int id)
        {
            var user = await _context.Usuarios.Include(u => u.Company).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Company == null)
            {
                throw new Exception("User not found.");
            }
            var provider = await _context.Prestadores.Where(p => p.Id == id && p.Company.UserId == userId)
                .Select(p => new Get_Provider_Dto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    PhoneNumber = p.PhoneNumber,
                    Services = p.Services.Select(s => new Get_Service_Dto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        DurationMinutes = s.DurationMinutes,
                        CompanyID = s.CompanyID
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (provider == null)
            {
                throw new Exception("Provider not found.");
            }
            return provider;
        }

        public async Task<bool> DeleteProvider(int userId, int id)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            var deleted = await _context.Prestadores
                .Where(p => p.Id == id && p.Company.UserId == userId)
                .ExecuteDeleteAsync();
            if (deleted == 0) return false;

            await _context.Usuarios
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.AvailableProviders, u => u.AvailableProviders + 1));

            await tx.CommitAsync();
            return true;
        }


    }
    }
