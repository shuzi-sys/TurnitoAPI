namespace TurnitoAPI.Models
{
    public struct WorkTime 
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
    public class ProviderWorkSchedule
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; } = null!;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
