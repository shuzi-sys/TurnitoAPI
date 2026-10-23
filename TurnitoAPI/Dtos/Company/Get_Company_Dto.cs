using TurnitoAPI.Dtos.Appointment;
using TurnitoAPI.Dtos.Provider;
using TurnitoAPI.Dtos.Service;
namespace TurnitoAPI.Dtos.Company
{
    public class Get_Company_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
