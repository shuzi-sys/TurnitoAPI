using System.ComponentModel.DataAnnotations;

namespace TurnitoAPI.Dtos.User
{
    public class Create_User_Dto
    {
        [Required, MinLength(5), MaxLength(20)]
        public string Username { get; set; } = null!;
        [Required, MinLength(8), MaxLength(25)]
        public string Password { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }
}
