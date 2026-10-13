using TurnitoAPI.Dtos.Appointment;
namespace TurnitoAPI.Services.Interfaces
{
    public interface IappointmentService
    {
        Task<ICollection<Get_Appointment_Dto>> GetAllAppointmentsForCompany(int userId, int companyId);
        Task<Get_Appointment_Dto> GetAppointmentById(int userId, int id);
        Task<Get_Appointment_Dto> CreateAppointment(Create_Appointment_Dto appointment);
        Task<Get_Appointment_Dto> UpdateAppointment(int userId, int id, Update_Appointment_Dto appointment);
        Task<bool> DeleteAppointment(int userId, int id);
    }
}
