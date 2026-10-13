namespace TurnitoAPI.Dtos.Appointment
{
    public class Update_Appointment_Dto
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? ServiceId { get; set; }
        public int? ProviderId { get; set; }
    }
}
