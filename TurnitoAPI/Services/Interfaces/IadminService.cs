using TurnitoAPI.Dtos.User;

namespace TurnitoAPI.Services.Interfaces
{
    public interface IadminService
    {
        public Task<User_Quotas_Dto> AddBasicPlan(int userId, int targetId);
        public Task<User_Quotas_Dto> RemovePlan(int userId, int targetId);
        public Task<User_Quotas_Dto> AddAvailableCompanies(int userId, int Amount, int targetId);
        public Task<User_Quotas_Dto> AddAvailableProviders(int userId, int Amount, int targetId);
        public Task<User_Quotas_Dto> AddAvailableServices(int userId, int Amount, int targetId);
        public Task<User_Quotas_Dto> AddAvailableAppointments(int userId, int Amount, int targetId);
    }
}
