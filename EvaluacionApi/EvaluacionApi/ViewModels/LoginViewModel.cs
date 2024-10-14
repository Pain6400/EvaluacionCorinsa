using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // Campo opcional para el código TOTP
        public string TOTPCode { get; set; }
    }
}
