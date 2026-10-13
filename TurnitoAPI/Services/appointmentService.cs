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

        public async Task<Get_Appointment_Dto> CreateAppointment(Create_Appointment_Dto appointment)
        {
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
            if (appointment.DateEnd != null)
            {
                existingAppointment.DateEnd = appointment.DateEnd.Value;
            }
            if (appointment.ServiceId != null)
            {
                existingAppointment.ServiceId = appointment.ServiceId.Value;
            }
            if (appointment.ProviderId != null)
            {
                existingAppointment.ProviderId = appointment.ProviderId.Value;
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
