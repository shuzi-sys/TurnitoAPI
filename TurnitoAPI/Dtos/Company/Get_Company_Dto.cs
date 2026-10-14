using TurnitoAPI.Dtos.Appointment;
using TurnitoAPI.Dtos.Provider;
using TurnitoAPI.Dtos.Service;
namespace TurnitoAPI.Dtos.Company
{
    public class Get_Company_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Get_Service_Dto> Services { get; set; }
        public ICollection<Get_Provider_Dto> Providers { get; set; }
        public ICollection<Get_Appointment_Dto> Appointments { get; set; }
    }
}
