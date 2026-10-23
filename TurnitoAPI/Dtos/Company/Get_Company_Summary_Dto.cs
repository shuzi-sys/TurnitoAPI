namespace TurnitoAPI.Dtos.Company
{
    public class Get_Company_Summary_Dto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ServicesCount { get; set; } = 0;
        public int ProvidersCount { get; set; } = 0;
        public int AppointmentsCount { get; set; } = 0;

    }
}
