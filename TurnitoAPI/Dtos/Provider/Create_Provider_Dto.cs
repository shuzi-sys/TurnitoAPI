namespace TurnitoAPI.Dtos.Provider
{
    public class Create_Provider_Dto
    {
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string? PhoneNumber { get; set; }
        public List<int> ServicesID { get; set; }
    }
}
