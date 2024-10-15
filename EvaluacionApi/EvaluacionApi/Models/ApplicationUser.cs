using Microsoft.AspNetCore.Identity;

namespace EvaluacionApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string? FotografiaUrl { get; set; }

        // Propiedad para almacenar la clave secreta de TOTP
        public string TOTPSecret { get; set; }

        // Relaciones
        public ICollection<ApplicationUserZone> ApplicationUserZones { get; set; }
        public ICollection<ApplicationUserRequestType> ApplicationUserRequestTypes { get; set; }
    }
}
