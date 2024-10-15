namespace EvaluacionApi.Models
{
    public class RequestType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ApplicationUserRequestType> ApplicationUserRequestTypes { get; set; }
    }
}
