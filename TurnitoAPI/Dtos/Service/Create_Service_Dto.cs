using System.ComponentModel.DataAnnotations;

namespace TurnitoAPI.Dtos.Service
{
    public class Create_Service_Dto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, Range(0,1440)]
        public int DurationMinutes { get; set; }
        [Required, Range(0, 99999999)]
        public double Price { get; set; }
        public int CompanyID { get; set; }
    }
}
