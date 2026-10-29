using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TurnitoAPI.Models
{
    [Index(nameof(CompanyID), nameof(Name), IsUnique=true)]
    public class Service
    {
        public int Id { get; set; }
        public int CompanyID { get; set; }
        public Company Company { get; set; } = null!;
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required, Range(0,1440)]
        public int DurationMinutes { get; set; }
        [Required, Range(0, 99999999)]
        public double Price { get; set; }
        public ICollection<Provider> Providers { get; set; } = new List<Provider>();

    }
}
