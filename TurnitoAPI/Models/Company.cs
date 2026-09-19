namespace TurnitoAPI.Models
{
    public class Company
    {
        public int id { get; set; }
        public string name { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Provider> Providers { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
