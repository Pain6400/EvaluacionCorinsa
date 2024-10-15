using System.ComponentModel.DataAnnotations;

namespace EvaluacionApi.ViewModels
{
    public class CreateSolicitudViewModel
    {
        [Required]
        public int TipoSolicitudId { get; set; }

        [Required]
        public int ZonaId { get; set; }

        public string Observaciones { get; set; }
    }
}
