namespace TurnitoAPI.Dtos.User
{
    public class Auth_Response_Dto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public User_Dto user { get; set; } = null!;
    }
}
