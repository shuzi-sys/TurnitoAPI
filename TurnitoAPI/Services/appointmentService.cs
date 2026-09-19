using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Appointment;

namespace TurnitoAPI.Services
{
    public class appointmentService
    {
        private readonly AppDbContext _context;
        public appointmentService(AppDbContext context) { this._context = context; }

        public async Task<ICollection<Appointment>> GetAllAppointmentsForCompany(int companyId)
        {
            return await _context.Turnos.Where(t => t.CompanyId == companyId).ToListAsync();
        }

        public async Task<Get_Appointment_Dto> GetAppointmentById(int id)
        {
            return await _context.Turnos.Where(t => t.Id == id)
                .Select(a => new Get_Appointment_Dto
                {
                    Id = a.Id,
                    DateStart = a.DateStart,
                    DateEnd = a.DateEnd,
                    CompanyId = a.CompanyId,
                    ServiceId = a.ServiceId,
                    ProviderId = a.ProviderId,
                    CompanyName = a.Company.name,
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

                    CompanyName = a.Company.name,
                    ServiceName = a.Service.Name,
                    ProviderName = a.Provider.Name,

                    ClientName = a.ClientName,
                    ClientNumber = a.ClientNumber
                }).FirstAsync();


            return createdAppointment;
        }

        public async Task<bool> DeleteAppointment(int id)
        {
            var appointment = await _context.Turnos.FindAsync(id);
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
