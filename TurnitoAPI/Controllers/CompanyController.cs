using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TurnitoAPI.Dtos.Company;
using TurnitoAPI.Models;
using TurnitoAPI.Services.Interfaces;

namespace TurnitoAPI.Controllers
{
    [Route("api/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IcompanyService _companyService;
        public CompanyController(IcompanyService companyService)
        {
            _companyService = companyService;
        }
        // proteger a adm
        [Authorize(Roles="Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCompanies()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var companies = await _companyService.GetAllCompanies(userId);
            return Ok(companies);
        }
        
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var company = await _companyService.GetCompanyById(userId, id);
            return Ok(company);
        }

        [Authorize]
        [HttpGet("{id:int")]
        public async Task<IActionResult> GetCompanySummaryById(int id) 
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var company = await _companyService.GetCompanySummaryById(userId, id);
            return Ok(company);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCompany(Create_Company_Dto companyDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var company = await _companyService.CreateCompany(userId, companyDto);
            if (company == null)
            {
                return BadRequest();
            }
            return Created(string.Empty, company);
        }

        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateCompany(int id, Update_Company_Dto companyDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var company = await _companyService.UpdateCompanyName(userId, id, companyDto);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _companyService.DeleteCompany(userId, id);
            return NoContent();
        }

    }
}
