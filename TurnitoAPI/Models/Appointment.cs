using System.ComponentModel.DataAnnotations;
using TurnitoAPI.Models;
namespace TurnitoAPI.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public int ProviderId { get; set; }
        public Provider Provider { get; set; } = null!;
        // You may manage these two as you want them (asking for a number is higly suggested though [i.e > cancelling an appointment]]!)
        public string? ClientName { get; set; }
        public string? ClientNumber { get; set; }
    }
}
