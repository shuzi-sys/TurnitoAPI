using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Appointment;
using TurnitoAPI.Services.Interfaces;
using TurnitoAPI.Exceptions;
namespace TurnitoAPI.Services
{
    public class appointmentService : IappointmentService
    {
        private readonly AppDbContext _context;
        public appointmentService(AppDbContext context) { this._context = context; }

        public async Task<ICollection<Get_Appointment_Dto>> GetAllAppointmentsForCompany(int userId, int companyId)
        {
            var ownsCompany = await _context.Empresas
                .AnyAsync(c => c.Id == companyId && c.UserId == userId);

            if (!ownsCompany)
            { return null; }

            return await _context.Turnos
                .AsNoTracking()
                .Where(a => a.CompanyId == companyId)
                .OrderBy(a => a.DateStart)
                .Select(a => new Get_Appointment_Dto
                {
                    Id = a.Id,
                    DateStart = a.DateStart,
                    DateEnd = a.DateEnd,
                    CompanyId = a.CompanyId,
                    ServiceId = a.ServiceId,
                    ProviderId = a.ProviderId,
                    CompanyName = a.Company.Name,
                    ServiceName = a.Service.Name,
                    ProviderName = a.Provider.Name,
                    ClientName = a.ClientName,
                    ClientNumber = a.ClientNumber
                })
                .ToListAsync();
        }

        public async Task<Get_Appointment_Dto> GetAppointmentById(int userId, int id)
        {
            var appointment = await _context.Turnos
                .AsNoTracking()
                .Where(t => t.Id == id && t.Company.UserId == userId)
                .Select(a => new Get_Appointment_Dto
                {
                    Id = a.Id,
                    DateStart = a.DateStart,
                    DateEnd = a.DateEnd,
                    CompanyId = a.CompanyId,
                    ServiceId = a.ServiceId,
                    ProviderId = a.ProviderId,
                    CompanyName = a.Company.Name,
                    ServiceName = a.Service.Name,
                    ProviderName = a.Provider.Name,
                    ClientName = a.ClientName,
                    ClientNumber = a.ClientNumber
                })
                .FirstOrDefaultAsync();

            return appointment ?? throw new NotFoundException("Appointment not found.");
        }

        public async Task<Get_Appointment_Dto> CreateAppointment(int userId, Create_Appointment_Dto dto)
        {
            // 1. check for ownership
            var ownsCompany = await _context.Empresas
                .AnyAsync(c => c.Id == dto.CompanyId && c.UserId == userId);
            if (!ownsCompany)
                throw new NotFoundException("Company not found");

            // 2. check if company offers such service
            var service = await _context.Servicios
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == dto.ServiceId && s.CompanyID == dto.CompanyId)
                ?? throw new NotFoundException("Service not found.");

            // 3. check if provider is from such company and offers such service
            var providerOk = await _context.Prestadores.AnyAsync(p =>
                p.Id == dto.ProviderId &&
                p.CompanyId == dto.CompanyId &&
                p.Services.Any(s => s.Id == dto.ServiceId));
            if (!providerOk)
                throw new BusinessRuleException("Specified provider does not exist, or does not provide such service.");

            // 4. fix timestamp for npgsql 
            var start = dto.DateStart.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(dto.DateStart, DateTimeKind.Utc)
                : dto.DateStart.ToUniversalTime();

            if (start <= DateTime.UtcNow)
                throw new BusinessRuleException("Appointment date must be future.");

            var end = start.AddMinutes(service.DurationMinutes);

            // 5. checking if it overlaps with another appointment
            var overlaps = await _context.Turnos.AnyAsync(a =>
                a.ProviderId == dto.ProviderId &&
                a.DateStart < end &&
                a.DateEnd > start);
            if (overlaps)
                throw new ConflictException("Provider already has an appointment at such date/time.");

            // 6. attomic insertion
            await using var tx = await _context.Database.BeginTransactionAsync();

            var updated = await _context.Usuarios
                .Where(u => u.Id == userId && u.AvailableAppointments > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    u => u.AvailableAppointments, u => u.AvailableAppointments - 1));
            if (updated == 0)
                throw new BusinessRuleException("No appointments available.");

            var newAppointment = new Appointment
            {
                DateStart = start,
                DateEnd = end,
                CompanyId = dto.CompanyId,
                ServiceId = dto.ServiceId,
                ProviderId = dto.ProviderId,
                ClientName = dto.ClientName.Trim(),
                ClientNumber = dto.ClientNumber.Trim()
            };
            _context.Turnos.Add(newAppointment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23P01" })
            {
                // race: another request took that datetime before you
                throw new ConflictException("Provider already has an appointment at such date/time.");
            }

            await tx.CommitAsync();

            return await _context.Turnos
                .AsNoTracking()
                .Where(a => a.Id == newAppointment.Id)
                .Select(a => new Get_Appointment_Dto
                {
                    Id = a.Id,
                    DateStart = a.DateStart,
                    DateEnd = a.DateEnd,
                    CompanyId = a.CompanyId,
                    ServiceId = a.ServiceId,
                    ProviderId = a.ProviderId,
                    CompanyName = a.Company.Name,
                    ServiceName = a.Service.Name,
                    ProviderName = a.Provider.Name,
                    ClientName = a.ClientName,
                    ClientNumber = a.ClientNumber
                })
                .FirstAsync();
        }

        // this is supossed to be accessed by the company [aka userId], not the person creating the appointment
        /* Anyone who uses this update appointment method should be using it with one of two scenarios:
         * 1 - Mark as completed (common)
         * 2 - Edit appointment details (which SHOULD notify the client with it's contact number)
         */
        public async Task<Get_Appointment_Dto> UpdateAppointment(int userId, int id, Update_Appointment_Dto dto)
        {
            var appointment = await _context.Turnos
                .FirstOrDefaultAsync(a => a.Id == id && a.Company.UserId == userId)
                ?? throw new NotFoundException("Appointment not found.");

            var finalServiceId = dto.ServiceId ?? appointment.ServiceId;
            var finalProviderId = dto.ProviderId ?? appointment.ProviderId;


            var service = await _context.Servicios
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == finalServiceId && s.CompanyID == appointment.CompanyId)
                ?? throw new BusinessRuleException("Company does not offer such service.");

            var providerOk = await _context.Prestadores.AnyAsync(p =>
                p.Id == finalProviderId &&
                p.CompanyId == appointment.CompanyId &&
                p.Services.Any(s => s.Id == finalServiceId));
            if (!providerOk)
                throw new BusinessRuleException("Specified provider does not exist or does not provide such service");

            // Fecha en UTC
            var start = appointment.DateStart;
            if (dto.DateStart != null)
            {
                start = dto.DateStart.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dto.DateStart.Value, DateTimeKind.Utc)
                    : dto.DateStart.Value.ToUniversalTime();

                if (start <= DateTime.UtcNow)
                    throw new BusinessRuleException("Appointment date must be future.");
            }

            var end = start.AddMinutes(service.DurationMinutes);

            // Solapamiento, excluyendo este mismo turno
            var overlaps = await _context.Turnos.AnyAsync(a =>
                a.Id != id &&
                a.ProviderId == finalProviderId &&
                a.DateStart < end &&
                a.DateEnd > start);
            if (overlaps)
                throw new ConflictException("Provider already has an appointment at such date/time.");

            appointment.DateStart = start;
            appointment.DateEnd = end;
            appointment.ServiceId = finalServiceId;
            appointment.ProviderId = finalProviderId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23P01" })
            {
                throw new ConflictException("Provider already has an appointment at such date/time.");
            }

            return await _context.Turnos
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new Get_Appointment_Dto
                {
                    Id = a.Id,
                    DateStart = a.DateStart,
                    DateEnd = a.DateEnd,
                    CompanyId = a.CompanyId,
                    ServiceId = a.ServiceId,
                    ProviderId = a.ProviderId,
                    CompanyName = a.Company.Name,
                    ServiceName = a.Service.Name,
                    ProviderName = a.Provider.Name,
                    ClientName = a.ClientName,
                    ClientNumber = a.ClientNumber
                })
                .FirstAsync();
        }
        public async Task DeleteAppointment(int userId, int id)
        {
            var appointment = await _context.Turnos
                .FirstOrDefaultAsync(a => a.Id == id && a.Company.UserId == userId)
                ?? throw new NotFoundException("Appointment not found.");

            await using var tx = await _context.Database.BeginTransactionAsync();

            _context.Turnos.Remove(appointment);
            await _context.SaveChangesAsync();

            await _context.Usuarios
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    u => u.AvailableAppointments, u => u.AvailableAppointments + 1));

            await tx.CommitAsync();
        }
    }
}
