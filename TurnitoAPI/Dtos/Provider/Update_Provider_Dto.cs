using System.ComponentModel.DataAnnotations;
using TurnitoAPI.Models;

namespace TurnitoAPI.Dtos.Provider
{
    public class Update_Provider_Dto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(25)]
        public string? PhoneNumber { get; set; }
        public Dictionary<DayOfWeek, List<WorkTime>>? ProviderWorkSchedule { get; set; }
        public List<int>? ServiceIds { get; set; } = new();
    }
}
