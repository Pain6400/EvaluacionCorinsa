namespace EvaluacionApi.Models
{
    public class ApplicationUserRequestType
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }
    }
}
