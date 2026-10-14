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
        
        public async Task<List<Get_Company_Dto>> GetAllCompanies(int userId)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null || user.isAdmin != true) 
            {
                return null;
            }
            var companies = await _context.Empresas.Select(c => new Get_Company_Dto
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
            return companies;
        }

        public async Task<Get_Company_Dto> GetCompanyById(int userId, int id)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            var company = await _context.Empresas.FindAsync(id);
            if (company == null || company.UserId != userId)
            {
                return null;
            }
            return new Get_Company_Dto
            {
                Id = company.Id,
                Name = company.Name
            };
        }


        // metele los dtos al dto del get-company y devolvelo cuando termines lo demas.
        public async Task<Get_Company_Dto> CreateCompany(int userId, Create_Company_Dto companyDto)
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
                UserId = userId,
                Name = companyDto.name
            };
            _context.Empresas.Add(company);
            await _context.SaveChangesAsync();
            return new Get_Company_Dto
            {
                Id = company.Id,
                Name = company.Name
            };
        }

        public async Task<Get_Company_Dto> UpdateCompanyName(int userId, int companyId, Update_Company_Dto  companyDto)
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
            var companyModified = await _context.Empresas.FindAsync(companyId);
            return new Get_Company_Dto
            {
                Id = companyModified.Id,
                Name = companyModified.Name
            };
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
