namespace EvaluacionApi.Models
{
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ApplicationUserZone> ApplicationUserZones { get; set; }
    }
}
