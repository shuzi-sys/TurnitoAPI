using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Company;
namespace TurnitoAPI.Services
{
    public class companyService : IcompanyService
    {
        private readonly AppDbContext _context;
        public companyService(AppDbContext context) { this._context = context; }
        
        public async Task<List<Company>> GetAllCompanies()
        {
            return await _context.Empresas.ToListAsync();
        }

        public async Task<Company> GetCompanyById(int id)
        {
            return await _context.Empresas.FindAsync(id);
        }


        // metele los dtos al dto del get-company y devolvelo cuando termines lo demas.
        public async Task<Company> CreateCompany(Create_Company_Dto companyDto)
        {
            var company = new Company
            {
                name = companyDto.name
            };
            _context.Empresas.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        // lo mismo aca
        public async Task<Company> UpdateCompany(int id, Create_Company_Dto companyDto)
        {
            var company = await _context.Empresas.FindAsync(id);
            if (company == null)
            {
                return null;
            }
            company.name = companyDto.name;
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> DeleteCompany(int id)
        {
            var company = await _context.Empresas.FindAsync(id);
            if (company == null)
            {
                return false;
            }
            _context.Empresas.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
