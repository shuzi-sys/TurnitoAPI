using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TurnitoAPI.Data;
using TurnitoAPI.Dtos.User;
using TurnitoAPI.Exceptions;
using TurnitoAPI.Models;
using TurnitoAPI.Services.Interfaces;

namespace TurnitoAPI.Services
{
    public class adminService : IadminService
    {
        private readonly AppDbContext _context;
        public adminService(AppDbContext context) { this._context = context; }
        /// <summary>
        /// Adds the most basic plan of usage (1 Company, 3 Providers, 15 Services & 400 Appointments)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="BusinessRuleException"></exception>
        public async Task<User_Quotas_Dto> AddBasicPlan(int userId, int targetId)
        {
            var isAdmin = await _context.Usuarios.AnyAsync(u => u.Id == userId && u.isAdmin);
            if (!isAdmin)
                throw new ForbiddenException("You dont have permission for this.");

            var exists = await _context.Usuarios.AnyAsync(u => u.Id == targetId);
            if (!exists)
                throw new NotFoundException("Target user not found");

            var updated = await _context.Usuarios
                .Where(u => u.Id == targetId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AvailableCompanies, u => u.AvailableCompanies +1)
                    .SetProperty(u => u.AvailableProviders, u => u.AvailableProviders +3)
                    .SetProperty(u => u.AvailableServices, u => u.AvailableServices +15)
                    .SetProperty(u => u.AvailableAppointments, u => u.AvailableAppointments+400)
                    );

            if (updated == 0)
                throw new BusinessRuleException("Unknown error while updating properties");

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == targetId)
                .Select(u => new User_Quotas_Dto
                {
                    Id = u.Id,
                    AvailableCompanies = u.AvailableCompanies,
                    AvailableProviders = u.AvailableProviders,
                    AvailableServices = u.AvailableServices,
                    AvailableAppointments = u.AvailableAppointments
                })
                .FirstAsync();
        }

        /// <summary>
        /// Removes every Available token for companies, services, appointments and providers, but doesnt delete them from the database.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="BusinessRuleException"></exception>
        public async Task<User_Quotas_Dto> RemovePlan(int userId, int targetId)
        {
            var isAdmin = await _context.Usuarios.AnyAsync(u => u.Id == userId && u.isAdmin);
            if (!isAdmin)
                throw new ForbiddenException("You dont have permission for this.");

            var exists = await _context.Usuarios.AnyAsync(u => u.Id == targetId);
            if (!exists)
                throw new NotFoundException("Target user not found");

            var updated = await _context.Usuarios
                .Where(u => u.Id == targetId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AvailableCompanies, 0)
                    .SetProperty(u => u.AvailableProviders, 0)
                    .SetProperty(u => u.AvailableServices, 0)
                    .SetProperty(u => u.AvailableAppointments, 0)
                    );

            if (updated == 0)
                throw new BusinessRuleException("Unknown error while updating properties");

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == targetId)
                .Select(u => new User_Quotas_Dto
                {
                    Id = u.Id,
                    AvailableCompanies = u.AvailableCompanies,
                    AvailableProviders = u.AvailableProviders,
                    AvailableServices = u.AvailableServices,
                    AvailableAppointments = u.AvailableAppointments
                })
                .FirstAsync();
        }

        /// <summary>
        /// Allows for addition of a specified amount non less or equal to zero
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        /// <exception cref="BusinessRuleException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<User_Quotas_Dto> AddAvailableCompanies(int userId, int Amount, int targetId) 
        {
            if (Amount <= 0) 
            {
                throw new BusinessRuleException("Specified amount is less than or equal to zero");
            }
            var isAdmin = await _context.Usuarios.AnyAsync(u => u.Id == userId && u.isAdmin);
            if (!isAdmin)
                throw new ForbiddenException("You dont have permission for this.");

            var exists = await _context.Usuarios.AnyAsync(u => u.Id == targetId);
            if (!exists)
                throw new NotFoundException("Target user not found");


            var updated = await _context.Usuarios
                .Where(u => u.Id == targetId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AvailableCompanies, u => u.AvailableCompanies + Amount)
                    );

            if (updated == 0)
                throw new BusinessRuleException("Unknown error while updating properties");

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == targetId)
                .Select(u => new User_Quotas_Dto
                {
                    Id = u.Id,
                    AvailableCompanies = u.AvailableCompanies,
                    AvailableProviders = u.AvailableProviders,
                    AvailableServices = u.AvailableServices,
                    AvailableAppointments = u.AvailableAppointments
                })
                .FirstAsync();
        }

        /// <summary>
        /// Allows for addition of a specified amount non less or equal to zero
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        /// <exception cref="BusinessRuleException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<User_Quotas_Dto> AddAvailableProviders(int userId, int Amount, int targetId)
        {
            if (Amount <= 0)
            {
                throw new BusinessRuleException("Specified amount is less than or equal to zero");
            }
            var isAdmin = await _context.Usuarios.AnyAsync(u => u.Id == userId && u.isAdmin);
            if (!isAdmin)
                throw new ForbiddenException("You dont have permission for this.");

            var exists = await _context.Usuarios.AnyAsync(u => u.Id == targetId);
            if (!exists)
                throw new NotFoundException("Target user not found");


            var updated = await _context.Usuarios
                .Where(u => u.Id == targetId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AvailableProviders, u => u.AvailableProviders + Amount)
                    );

            if (updated == 0)
                throw new BusinessRuleException("Unknown error while updating properties");

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == targetId)
                .Select(u => new User_Quotas_Dto
                {
                    Id = u.Id,
                    AvailableCompanies = u.AvailableCompanies,
                    AvailableProviders = u.AvailableProviders,
                    AvailableServices = u.AvailableServices,
                    AvailableAppointments = u.AvailableAppointments
                })
                .FirstAsync();
        }

        /// <summary>
        /// Allows for addition of a specified amount non less or equal to zero
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        /// <exception cref="BusinessRuleException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<User_Quotas_Dto> AddAvailableServices(int userId, int Amount, int targetId)
        {
            if (Amount <= 0)
            {
                throw new BusinessRuleException("Specified amount is less than or equal to zero");
            }
            var isAdmin = await _context.Usuarios.AnyAsync(u => u.Id == userId && u.isAdmin);
            if (!isAdmin)
                throw new ForbiddenException("You dont have permission for this.");

            var exists = await _context.Usuarios.AnyAsync(u => u.Id == targetId);
            if (!exists)
                throw new NotFoundException("Target user not found");


            var updated = await _context.Usuarios
                .Where(u => u.Id == targetId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AvailableServices, u => u.AvailableServices + Amount)
                    );

            if (updated == 0)
                throw new BusinessRuleException("Unknown error while updating properties");

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == targetId)
                .Select(u => new User_Quotas_Dto
                {
                    Id = u.Id,
                    AvailableCompanies = u.AvailableCompanies,
                    AvailableProviders = u.AvailableProviders,
                    AvailableServices = u.AvailableServices,
                    AvailableAppointments = u.AvailableAppointments
                })
                .FirstAsync();
        }

        /// <summary>
        /// Allows for addition of a specified amount non less or equal to zero
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        /// <exception cref="BusinessRuleException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<User_Quotas_Dto> AddAvailableAppointments(int userId, int Amount, int targetId)
        {
            if (Amount <= 0)
            {
                throw new BusinessRuleException("Specified amount is less than or equal to zero");
            }
            var isAdmin = await _context.Usuarios.AnyAsync(u => u.Id == userId && u.isAdmin);
            if (!isAdmin)
                throw new ForbiddenException("You dont have permission for this.");

            var exists = await _context.Usuarios.AnyAsync(u => u.Id == targetId);
            if (!exists)
                throw new NotFoundException("Target user not found");


            var updated = await _context.Usuarios
                .Where(u => u.Id == targetId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.AvailableAppointments, u => u.AvailableAppointments + Amount)
                    );

            if (updated == 0)
                throw new BusinessRuleException("Unknown error while updating properties");

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == targetId)
                .Select(u => new User_Quotas_Dto
                {
                    Id = u.Id,
                    AvailableCompanies = u.AvailableCompanies,
                    AvailableProviders = u.AvailableProviders,
                    AvailableServices = u.AvailableServices,
                    AvailableAppointments = u.AvailableAppointments
                })
                .FirstAsync();
        }

    }
}
