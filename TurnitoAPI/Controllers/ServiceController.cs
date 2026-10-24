using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        private readonly IserviceService _serviceService;
        public ServiceController( IserviceService serviceService)
        {
            _serviceService = serviceService;
        }

        [EnableRateLimiting("write")]
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

        [EnableRateLimiting("read")]
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

        [EnableRateLimiting("write")]
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
