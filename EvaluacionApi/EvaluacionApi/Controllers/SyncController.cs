using EvaluacionApi.Data;
using EvaluacionApi.Models;
using EvaluacionApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EvaluacionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SyncController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SyncController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Sync/UploadData
        [HttpPost("UploadData")]
        public async Task<IActionResult> UploadData([FromBody] List<SolicitudViewModel> solicitudes)
        {
            try
            {
                // Obtener el ID del usuario desde el token JWT
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }
                var usuarioId = userId;

                foreach (var solicitud in solicitudes)
                {
                    // Asignar automáticamente el usuarioId, zonaId y tipoSolicitudId
                    var nuevaSolicitud = new Solicitud
                    {
                        UsuarioId = usuarioId,
                        Observaciones = solicitud.Observaciones,
                        FechaCreacion = DateTime.UtcNow,
                        Aprobada = false,
                        ZonaId = ObtenerZonaIdPorDefecto(usuarioId),  // Puedes cambiar esta lógica
                        TipoSolicitudId = ObtenerTipoSolicitudPorDefecto(usuarioId), // Puedes cambiar esta lógica
                    };

                    _context.Solicitudes.Add(nuevaSolicitud);
                }

                await _context.SaveChangesAsync();

                return Ok("Datos subidos y guardados correctamente.");
            }
            catch (Exception ex)
            {
                // Manejo del error
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Sync/DownloadData
        [HttpGet("DownloadData")]
        public async Task<IActionResult> DownloadData()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Obtener todas las solicitudes de la base de datos.
                var solicitudes = await _context.Solicitudes.Where(w => w.UsuarioId == userId).ToListAsync();
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private int ObtenerZonaIdPorDefecto(string usuarioId)
        {
            return _context.ApplicationUserZones.FirstOrDefault(z => z.UserId == usuarioId).ZoneId; 
        }

        private int ObtenerTipoSolicitudPorDefecto(string usuarioId)
        {
            return _context.ApplicationUserRequestTypes.FirstOrDefault(t => t.UserId == usuarioId).RequestTypeId; 
        }

    }
}
