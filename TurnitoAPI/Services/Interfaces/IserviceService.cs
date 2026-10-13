using TurnitoAPI.Dtos.Service;
namespace TurnitoAPI.Services.Interfaces
{
    public interface IserviceService
    {
        Task<Get_Service_Dto> CreateServiceAsync(int userId, Create_Service_Dto dto);
        Task<IEnumerable<Get_Service_Dto>> GetServicesByCompanyIdAsync(int userId, int id);
        Task<bool> DeleteServiceById(int userId, int serviceId);
    }
}
