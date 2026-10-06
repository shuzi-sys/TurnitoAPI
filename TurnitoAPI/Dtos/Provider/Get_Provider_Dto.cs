using TurnitoAPI.Dtos.Service;
using TurnitoAPI.Dtos.Appointment;
namespace TurnitoAPI.Dtos.Provider
{
    public class Get_Provider_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string? PhoneNumber { get; set; }
        public ICollection<Get_Service_Dto> Services { get; set; }
    }
}
