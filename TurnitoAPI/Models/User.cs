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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Company>? Company { get; set; } = new List<Company>(); 
    }
}
