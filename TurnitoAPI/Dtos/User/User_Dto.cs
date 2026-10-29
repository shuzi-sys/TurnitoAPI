using TurnitoAPI.Dtos.Company;
namespace TurnitoAPI.Dtos.User
{
    public class User_Quotas_Dto
    {
        public int Id { get; set; }
        public int AvailableCompanies { get; set; } = 0;
        public int AvailableProviders { get; set; } = 0;
        public int AvailableServices { get; set; } = 0;
        public int AvailableAppointments { get; set; } = 0;
        public bool isAdmin { get; set; }
    }
}
