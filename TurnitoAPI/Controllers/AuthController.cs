using Microsoft.AspNetCore.Mvc;
using TurnitoAPI.Dtos.User;
using TurnitoAPI.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;

namespace TurnitoAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [EnableRateLimiting("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IauthService _authService;
        public AuthController(IauthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Create_User_Dto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result != null)
            {
                return Created(string.Empty, result);
            }
            return BadRequest();
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login_User_Dto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized("Credenciales invalidas.");
        }
    }
}
