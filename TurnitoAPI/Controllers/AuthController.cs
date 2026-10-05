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

        /*
        [HttpPost("register")]
        public async Task<IActionResult> Register(Create_User_Dto dto)
        {

        }
        */

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
