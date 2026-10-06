using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Data;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.Company;
using TurnitoAPI.Services.Interfaces;
namespace TurnitoAPI.Services
{
    public class companyService : IcompanyService
    {
        private readonly AppDbContext _context;
        public companyService(AppDbContext context) { this._context = context; }
        
        public async Task<List<Company>> GetAllCompanies(int userId)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null || user.isAdmin != true) 
            {
                return null;
            }
            return await _context.Empresas.ToListAsync();
        }

        public async Task<Company> GetCompanyById(int userId, int id)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            var company = await _context.Empresas.FindAsync(id);
            if (company == null || company.UserId != userId)
            {
                return null;
            }
            return company;
        }


        // metele los dtos al dto del get-company y devolvelo cuando termines lo demas.
        public async Task<Company> CreateCompany(int userId, Create_Company_Dto companyDto)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
            {
                return null;
            }
            if (user.AvailableCompanies <= 0)
            {
                return null;
            }
            var company = new Company
            {
                Name = companyDto.name
            };
            _context.Empresas.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company> UpdateCompanyName(int userId, int companyId, Update_Company_Dto  companyDto)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
            {
                return null;
            }
            var company = await _context.Empresas.FindAsync(companyId);
            if (company == null)
            {
                return null;
            }
            if (company.UserId != userId)
            {
                return null;
            }
            company.Name = companyDto.Name;
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> DeleteCompany(int userId, int id)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            var company = await _context.Empresas.FindAsync(id);
            if (company == null)
            {
                return false;
            }
            if (company.UserId != userId)
            {
                return false;
            }
            _context.Empresas.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
