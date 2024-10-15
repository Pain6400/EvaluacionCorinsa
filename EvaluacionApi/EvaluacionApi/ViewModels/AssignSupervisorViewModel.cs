using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.ViewModels
{
    public class AssignSupervisorViewModel
    {
        [Required]
        [EmailAddress]
        public string SupervisorEmail { get; set; }

        [Required]
        public List<int> ZoneIds { get; set; }

        [Required]
        public List<int> RequestTypeIds { get; set; }
    }
}
