using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TurnitoAPI.Models;
using TurnitoAPI.Dtos.User;

namespace TurnitoAPI.Services
{
    public class tokenService
    {
        private readonly IConfiguration _configuration;
        public tokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Auth_Response_Dto CreateToken(User user) 
        {
            var expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiresInMinutes"));

            var Claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Mail)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: Claims,
                expires: expires,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new Auth_Response_Dto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt=expires
            };
        }


    }
}
