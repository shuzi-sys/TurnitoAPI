using TurnitoAPI.Data;
using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Dtos.User;
using TurnitoAPI.Models;
using BCrypt.Net;
using TurnitoAPI.Services.Interfaces;

namespace TurnitoAPI.Services
{
    public class authService : IauthService
    {
        private readonly AppDbContext _context;
        private readonly ItokenService _token;
        public authService(AppDbContext context, ItokenService token)
        {
            _context = context;
            _token = token;
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            return await _context.Usuarios.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Mail == email);
        }

        public async Task<Auth_Response_Dto> RegisterAsync(Create_User_Dto dto)
        {
            if (IsUsernameTaken(dto.Username).Result)
            {
                throw new Exception("Username is already taken.");
            }
            if (IsEmailTaken(dto.Email).Result)
            {
                throw new Exception("Email is already taken.");
            }

            // Continue with user registration logic
            var user = new User
            {
                Username = dto.Username,
                Mail = dto.Email,
                Pwhash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                isAdmin = false
            };

            _context.Usuarios.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new Exception("Username or email taken");
            }
            // Create and return authentication response
            return await _token.CreateToken(user);
        }

        public async Task<Auth_Response_Dto> LoginAsync(Login_User_Dto dto)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Pwhash))
            {
                throw new Exception("Invalid password");
            }

            return await _token.CreateToken(user);
        }
    }
}
