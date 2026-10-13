using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TurnitoAPI.Data;
using TurnitoAPI.Dtos.Service;
using TurnitoAPI.Models;
using TurnitoAPI.Services.Interfaces;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TurnitoAPI.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IserviceService _serviceService;
        public ServiceController(AppDbContext context, IserviceService serviceService)
        {
            _context = context;
            _serviceService = serviceService;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ServiceController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ServiceController>
        [HttpPost("service")]
        public async Task<IActionResult> CreateService(Create_Service_Dto serviceDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = await _serviceService.CreateServiceAsync(userId, serviceDto);
            if (service == null)
            {
                return BadRequest("Failed to create service.");
            }
            return Created(string.Empty, service);
        }

        [HttpGet("company/{companyId:int}/service")]
        public async Task<IActionResult> GetServicesByCompanyId(int companyId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var services = await _serviceService.GetServicesByCompanyIdAsync(userId, companyId);
            if (services == null)
            {
                return NotFound("Company not found or you do not have access to it.");
            }
            return Ok(services);
        }

        
        [HttpDelete("service/{id:int}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _serviceService.DeleteServiceById(userId, id);
            if (!result)
            {
                return NotFound("Service not found or you do not have access to it.");
            }
            return NoContent();
        }
    }
}

<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
