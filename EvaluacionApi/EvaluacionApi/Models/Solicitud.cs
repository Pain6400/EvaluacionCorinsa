using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.Models
{
    public class Solicitud
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; } // FK a ApplicationUser

        // Navegación a ApplicationUser
        public ApplicationUser Usuario { get; set; }

        [Required]
        public int ZonaId { get; set; }
        public Zone Zona { get; set; }

        [Required]
        public int TipoSolicitudId { get; set; }
        public RequestType TipoSolicitud { get; set; }

        public string Observaciones { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public bool Aprobada { get; set; }

        public DateTime? FechaRespuesta { get; set; } // Nueva propiedad
    }
}
