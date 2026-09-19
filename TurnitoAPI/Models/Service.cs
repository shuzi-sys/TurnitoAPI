namespace TurnitoAPI.Models
{
    public class Service
    {
        public int id { get; set; }
        public int CompanyID { get; set; }
        public Company Company { get; set; }
        public string Name { get; set; }
        public int DurationMinutes { get; set; }
        public ICollection<Provider> Providers { get; set; }

    }
}
