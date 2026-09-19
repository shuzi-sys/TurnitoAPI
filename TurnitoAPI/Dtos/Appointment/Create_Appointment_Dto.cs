namespace TurnitoAPI.Dtos.Appointment
{
    public class Create_Appointment_Dto
    {
        public DateTime DateStart { get; set; }
        public int CompanyId { get; set; }
        public int ServiceId { get; set; }
        public int ProviderId { get; set; }

        public string ClientName { get; set; }
        public string ClientNumber { get; set; }

    }
}
