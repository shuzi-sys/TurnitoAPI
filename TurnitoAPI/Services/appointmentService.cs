using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Appointment;
using TurnitoAPI.Services.Interfaces;
namespace TurnitoAPI.Services
{
    public class appointmentService : IappointmentService
    {
        private readonly AppDbContext _context;
        public appointmentService(AppDbContext context) { this._context = context; }

        public async Task<ICollection<Get_Appointment_Dto>> GetAllAppointmentsForCompany(int userId, int companyId)
        {
            var Appointments = await _context.Turnos
                .Where(a=> a.CompanyId == companyId && a.Company.UserId == userId)
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
            return Appointments;
        }

        public async Task<Get_Appointment_Dto> GetAppointmentById(int userId, int id)
        {
            return await _context.Turnos.Where(t => t.Id == id && t.Company.UserId == userId)
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
                }).FirstAsync();
        }

        public async Task<Get_Appointment_Dto> CreateAppointment(int userId, Create_Appointment_Dto appointment)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null) { throw new Exception("Invalid user"); }
            if (user.AvailableAppointments <= 0) 
            {
                throw new Exception("You've hit the limit of appointments");
            }
            var newAppointment = new Appointment
            {
                DateStart = appointment.DateStart,
                CompanyId = appointment.CompanyId,
                ServiceId = appointment.ServiceId,
                ProviderId = appointment.ProviderId,
                ClientName = appointment.ClientName,
                ClientNumber = appointment.ClientNumber
            };
            _context.Turnos.Add(newAppointment);
            await _context.SaveChangesAsync();
            var createdAppointment = await _context.Turnos.Where(a => a.Id == newAppointment.Id)
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
                }).FirstAsync();


            return createdAppointment;
        }

        // this is supossed to be accessed by the company [aka userId], not the person creating the appointment
        /* Anyone who uses this update appointment method should be using it with one of two scenarios:
         * 1 - Mark as completed (common)
         * 2 - Edit appointment details (which SHOULD notify the client with it's contact number)
         */
        public async Task<Get_Appointment_Dto> UpdateAppointment(int userId, int id, Update_Appointment_Dto appointment)
        {

            var existingAppointment = await _context.Turnos
                .FirstOrDefaultAsync(a=>a.Id == id
                                     &&a.Company.UserId == userId);
            if (existingAppointment == null) {
                return null;
            }
            if (appointment.DateStart != null)
            {
                existingAppointment.DateStart = appointment.DateStart.Value;
            }
            // add check for "newer than datestart"
            if (appointment.DateEnd != null)
            {
                existingAppointment.DateEnd = appointment.DateEnd.Value;
            }
            if (appointment.ServiceId != null)
            {
                var serviceBelongsToCompany  = await _context.Servicios.AnyAsync(s =>
                s.Id == appointment.ServiceId.Value && 
                s.CompanyID == existingAppointment.CompanyId);
                if (!serviceBelongsToCompany)
                {
                    throw new Exception("Service is not owned by such company.");
                }
                existingAppointment.ServiceId = appointment.ServiceId.Value;
            }
            if (appointment.ProviderId != null)
            {
                var providerBelongsToCompany = await _context.Prestadores.AnyAsync(p =>
                    p.Id == appointment.ProviderId.Value &&
                    p.CompanyId == existingAppointment.CompanyId);

                if (!providerBelongsToCompany)
                {
                    throw new Exception("Provider doesnt belong to such company.");
                }
                existingAppointment.ProviderId = appointment.ProviderId.Value;
            }

            var finalServiceId = appointment.ServiceId ?? existingAppointment.ServiceId;
            var finalProviderId = appointment.ProviderId ?? existingAppointment.ProviderId;

            var providerOffersService = await _context.Prestadores.AnyAsync(p =>
                p.Id == finalProviderId &&
                p.Services.Any(s => s.Id == finalServiceId));

            if (!providerOffersService)
            {
                throw new Exception("The selected provider doesnt offer such service");
            }
            await _context.SaveChangesAsync();
            var updatedAppointment = await _context.Turnos.Where(a => a.Id == existingAppointment.Id)
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
                }).FirstAsync();
            return updatedAppointment;
                
        }
        public async Task<bool> DeleteAppointment(int userId, int id)
        {
            var appointment = await _context.Turnos.FirstOrDefaultAsync(a => a.Id == id && a.Company.UserId == userId);
            if (appointment == null)
            {
                return false;
            }
            _context.Turnos.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
