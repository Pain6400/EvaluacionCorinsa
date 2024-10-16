using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.ViewModels
{
    public class UpdateSolicitudViewModel
    {
        [Required]
        public int ZonaId { get; set; }

        [Required]
        public int TipoSolicitudId { get; set; }

        public string Observaciones { get; set; }

        [Required]
        public bool Aprobada { get; set; }
    }
}
