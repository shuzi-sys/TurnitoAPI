namespace TurnitoAPI.Dtos.Service
{
    public class Get_Service_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationMinutes { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
}
