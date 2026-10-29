using System.ComponentModel.DataAnnotations;

namespace TurnitoAPI.Models
{
    public class Provider
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        [MaxLength(25)]
        public string? PhoneNumber { get; set; }
        public ICollection<ProviderWorkSchedule> Schedules { get; set; } = new List<ProviderWorkSchedule>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
