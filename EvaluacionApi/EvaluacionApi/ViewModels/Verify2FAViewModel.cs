using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.ViewModels
{
    public class Verify2FAViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string TOTPCode { get; set; }
    }
}
