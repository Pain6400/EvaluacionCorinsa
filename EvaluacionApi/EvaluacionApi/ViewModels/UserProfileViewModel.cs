using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.ViewModels
{
    public class UserProfileViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombres { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string Apellidos { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El género no puede exceder los 20 caracteres.")]
        public string Genero { get; set; }
    }
}
