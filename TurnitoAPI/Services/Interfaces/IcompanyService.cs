using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Company;
namespace TurnitoAPI.Services.Interfaces
{
    public interface IcompanyService
    {
        public Task<List<Get_Company_Dto>> GetAllCompanies(int userId);
        public Task<Get_Company_Dto> GetCompanyById(int userId, int id);
        public Task<ICollection<Get_Company_Dto>> GetUserCompanies(int userId);
        public Task<Get_Company_Summary_Dto> GetCompanySummaryById(int userId, int id);
        public Task<ICollection<Get_Company_Summary_Dto>> GetUserCompaniesSummary(int userId);
        public Task<Get_Company_Dto> CreateCompany(int userId, Create_Company_Dto companyDto);
        public Task<Get_Company_Dto> UpdateCompanyName(int userId, int id, Update_Company_Dto companyDto);
        public Task DeleteCompany(int userId, int id);
    }
}
