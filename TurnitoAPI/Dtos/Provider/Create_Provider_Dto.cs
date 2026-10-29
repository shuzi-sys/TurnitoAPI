using System.ComponentModel.DataAnnotations;
using TurnitoAPI.Models;
namespace TurnitoAPI.Dtos.Provider
{
    public class Create_Provider_Dto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        public int CompanyId { get; set; }
        [MaxLength(25)]
        public string? PhoneNumber { get; set; }
        public Dictionary<DayOfWeek, List<WorkTime>>? ProviderWorkSchedule { get; set; }
        public List<int> ServicesID { get; set; } = new();
    }
}
