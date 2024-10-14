namespace EvaluacionApi.Models
{
    public class Solicitud
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Zona { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Aprobada { get; set; }
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }
    }
}
