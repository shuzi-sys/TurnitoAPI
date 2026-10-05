using Microsoft.EntityFrameworkCore;

namespace TurnitoAPI.Models
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Mail), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string Pwhash { get; set; } = null!;
        public bool isAdmin { get; set; } = false;
        public bool isPremium { get; set; } = false;
        public bool isTrial { get; set; } = false;
        public int AvailableCompanies { get; set; } = 0;
        public int AvailableProviders { get; set; } = 0;
        public int AvailableServices { get; set; } = 0;
        public int AvailableAppointments { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Company>? Company { get; set; } = new List<Company>(); 
    }
}
