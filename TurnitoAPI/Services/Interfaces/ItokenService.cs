using TurnitoAPI.Dtos.User;
using TurnitoAPI.Models;
namespace TurnitoAPI.Services.Interfaces
{
    public interface ItokenService
    {
        Task<Auth_Response_Dto> CreateToken(User user);
    }
}
