using System.ComponentModel.DataAnnotations;
namespace TurnitoAPI.Dtos.User
{
    public class Login_User_Dto
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
