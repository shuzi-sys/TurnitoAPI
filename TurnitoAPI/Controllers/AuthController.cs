using Microsoft.AspNetCore.Mvc;
using TurnitoAPI.Dtos.User;
using TurnitoAPI.Services.Interfaces;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TurnitoAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
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
