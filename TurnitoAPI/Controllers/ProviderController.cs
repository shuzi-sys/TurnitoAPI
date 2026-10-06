using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TurnitoAPI.Dtos.Provider;
using TurnitoAPI.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TurnitoAPI.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ProviderController : ControllerBase
    {
        private readonly IproviderService _providerService;
        public ProviderController(IproviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpPost("providers")]
        public async Task<IActionResult> CreateProvider(Create_Provider_Dto providerDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var provider = await _providerService.CreateProvider(userId, providerDto);
            if (provider == null)
            {
                return BadRequest();
            }
            return Created(string.Empty, provider);
        }

        // GET api/<ValuesController>/5
        [HttpPatch("providers/{id:int}")]
        public async Task<IActionResult> UpdateProvider(int id, Update_Provider_Dto providerDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var provider = await _providerService.UpdateProvider(userId, id, providerDto);
            if (provider == null)
            {
                return NotFound();
            }
            return Ok(provider);
        }

        [HttpPost("providers/{providerId:int}/services/")]
        public async Task<IActionResult> AddServiceToProvider(int providerId, Update_Provider_Services_Dto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var provider = await _providerService.AddServiceToProvider(userId, providerId, dto.ServiceIds);
            if (provider == null)
            {
                return NotFound();
            }
            return Ok(provider);
        }

        [HttpPost("providers/{providerId:int}/services")]
        public async Task<IActionResult> RemoveServiceFromProvider(int providerId, Update_Provider_Services_Dto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var provider = await _providerService.RemoveServiceFromProvider(userId, providerId, dto.ServiceIds);
            if (provider == null)
            {
                return NotFound();
            }
            return Ok(provider);
        }

        [HttpGet("companies/{companyId:int}/providers")]
        public async Task<IActionResult> GetAllProvidersForCompany(int companyId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var providers = await _providerService.GetAllProvidersForCompany(userId, companyId);
            return Ok(providers);
        }

        [HttpGet("providers/{providerId:int}")]
        public async Task<IActionResult> GetProviderById(int providerId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var provider = await _providerService.GetProviderById(userId, providerId);
            if (provider == null)
            {
                return NotFound();
            }
            return Ok(provider);
        }

        [HttpDelete("providers/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _providerService.DeleteProvider(userId, id);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
