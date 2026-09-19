namespace TurnitoAPI.Dtos.Company
{
    public class Get_Company_Dto
    {
        public string name { get; set; }
        public ICollection<//Servicedto> Services { get; set; }
        public ICollection<//Providerdto> Providers { get; set; }
        public ICollection<//Appointmentdto> Appointments { get; set; }
    }
}
