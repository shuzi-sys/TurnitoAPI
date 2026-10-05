using TurnitoAPI.Dtos.User;
namespace TurnitoAPI.Services.Interfaces
{
    public interface IauthService
    {
        Task<Auth_Response_Dto> LoginAsync(Login_User_Dto dto);
        Task<Auth_Response_Dto> RegisterAsync(Create_User_Dto dto);
    }
}
