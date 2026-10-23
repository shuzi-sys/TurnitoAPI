using System.ComponentModel.DataAnnotations;

namespace TurnitoAPI.Dtos.Service
{
    public class Create_Service_Dto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Range(1,1440)]
        public int DurationMinutes { get; set; }
        public int CompanyID { get; set; }
    }
}
