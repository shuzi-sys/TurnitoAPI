namespace TurnitoAPI.Dtos.User
{
    public class User_Dto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public bool isAdmin { get; set; }
    }
}
