using TurnitoAPI.Dtos.Service;
namespace TurnitoAPI.Dtos.Provider
{
    public class Get_Provider_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<Get_Service_Summary_Dto> Services { get; set; }
    }
}
