using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Company;
namespace TurnitoAPI.Services.Interfaces
{
    public interface IcompanyService
    {
        public Task<List<Company>> GetAllCompanies();
        public Task<Company> GetCompanyById(int userId, int id);
        public Task<Company> CreateCompany(int userId, Create_Company_Dto companyDto);
        public Task<Company> UpdateCompanyName(int userId, int id, Update_Company_Dto companyDto);
        public Task<bool> DeleteCompany(int userId, int id);
    }
}
