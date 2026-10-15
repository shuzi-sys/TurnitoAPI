using TurnitoAPI.Data;
using Microsoft.EntityFrameworkCore;
using TurnitoAPI.Dtos.User;
using TurnitoAPI.Models;
using TurnitoAPI.Exceptions;
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

       

        public async Task<Auth_Response_Dto> RegisterAsync(Create_User_Dto dto)
        {
            var mail = dto.Email.Trim().ToLowerInvariant();
            var username = dto.Username.Trim();
            var taken = await _context.Usuarios
                .AnyAsync(u => u.Username == dto.Username || u.Mail == mail);
            if (taken) 
            {
                throw new ConflictException("Username or email already in use");
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
            // race conditions
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("Username or email already in use");
            }
            // Create and return authentication response
            return await _token.CreateToken(user);
        }

        public async Task<Auth_Response_Dto> LoginAsync(Login_User_Dto dto)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Username == dto.Username);
            var pw = BCrypt.Net.BCrypt.Verify(dto.Password, user.Pwhash);
            if (user == null || !pw) { throw new ConflictException("Wrong username/password"); }
            return await _token.CreateToken(user);
        }
    }
}
