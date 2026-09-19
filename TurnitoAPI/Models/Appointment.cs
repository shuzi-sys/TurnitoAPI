using TurnitoAPI.Models;
namespace TurnitoAPI.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public string ClientName { get; set; }
        public string ClientNumber { get; set; }
    }
}
