using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.ViewModels
{
    public class AssignRoleViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
