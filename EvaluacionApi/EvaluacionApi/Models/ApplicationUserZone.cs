namespace EvaluacionApi.Models
{
    public class ApplicationUserZone
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
    }
}
