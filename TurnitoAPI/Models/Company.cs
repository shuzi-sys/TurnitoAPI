namespace TurnitoAPI.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Provider> Providers { get; set; } = new List<Provider>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
