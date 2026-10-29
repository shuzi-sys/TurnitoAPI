namespace TurnitoAPI.Dtos.Appointment
{
    public class Get_Appointment_Dto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int ProviderId { get; set; }
        public int CompanyId { get; set; }
        public string ServiceName { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string CompanyName { get; set; }
        public string ClientName { get; set; } = null!;
        public string ClientNumber { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

    }
}
