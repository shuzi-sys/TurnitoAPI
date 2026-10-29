using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Service;
using TurnitoAPI.Dtos.Appointment;
namespace TurnitoAPI.Dtos.Provider
{
    public class Get_Provider_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CompanyId { get; set; }
        public string? PhoneNumber { get; set; }
        public Dictionary<DayOfWeek, TimeOnly>? ProviderWorkSchedule { get; set; } = new();
        public ICollection<Get_Service_Dto> Services { get; set; } = new List<Get_Service_Dto>();
    }
}
