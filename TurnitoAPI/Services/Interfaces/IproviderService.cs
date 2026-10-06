using TurnitoAPI.Dtos.Provider;
namespace TurnitoAPI.Services.Interfaces
{
    public interface IproviderService
    {
        public Task<Get_Provider_Dto> CreateProvider(int userId, Create_Provider_Dto dto);
        public Task<Get_Provider_Dto> UpdateProvider(int userId, int id, Update_Provider_Dto dto);
    }
}
