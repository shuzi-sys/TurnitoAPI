using TurnitoAPI.Dtos.Provider;
namespace TurnitoAPI.Services.Interfaces
{
    public interface IproviderService
    {
        public Task<Get_Provider_Dto> CreateProvider(int userId, Create_Provider_Dto dto);
        public Task<Get_Provider_Dto> UpdateProvider(int userId, int id, Update_Provider_Dto dto);
        public Task<Get_Provider_Dto> AddServiceToProvider(int userId, int providerId, List<int> serviceIds);
        public Task<Get_Provider_Dto> RemoveServiceFromProvider(int userId, int providerId, List<int> serviceIds);
        public Task<List<Get_Provider_Dto>> GetAllProvidersForCompany(int userId, int companyId);
        public Task<Get_Provider_Dto> GetProviderById(int userId, int providerId);
        public Task<bool> DeleteProvider(int userId, int providerid);
    }
}
