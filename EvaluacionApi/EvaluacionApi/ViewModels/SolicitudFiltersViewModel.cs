namespace EvaluacionApi.ViewModels
{
    public class SolicitudFiltersViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } // "Approved" o "NotApproved"
    }
}
