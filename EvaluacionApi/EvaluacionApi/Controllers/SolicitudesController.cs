using Microsoft.AspNetCore.Mvc;
using EvaluacionApi.Data;
using EvaluacionApi.Authorization;
using EvaluacionApi.Models;
using EvaluacionApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using static QRCoder.PayloadGenerator;

namespace EvaluacionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación
    public class SolicitudesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SolicitudesController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        // GET: api/Solicitudes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solicitud>>> GetSolicitudes()
        {
            return await _context.Solicitudes.Include(s => s.Zona).Include(s => s.TipoSolicitud).ToListAsync();
        }

        // GET: api/Solicitudes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Solicitud>> GetSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.Include(s => s.Zona).Include(s => s.TipoSolicitud).FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            return solicitud;
        }

        // POST: api/Solicitudes
        [HttpPost]
        public async Task<ActionResult<Solicitud>> CreateSolicitud([FromBody] CreateSolicitudViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Obtener el UserId desde los claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Usuario no autenticado.");
            }

            // Verificar que ZonaId y TipoSolicitudId existen
            var zonaExists = await _context.Zones.AnyAsync(z => z.Id == model.ZonaId);
            if (!zonaExists)
            {
                return BadRequest("Zona inválida.");
            }

            var tipoSolicitudExists = await _context.RequestTypes.AnyAsync(rt => rt.Id == model.TipoSolicitudId);
            if (!tipoSolicitudExists)
            {
                return BadRequest("Tipo de solicitud inválido.");
            }

            // Crear la solicitud con el UserId correcto
            var solicitud = new Solicitud
            {
                TipoSolicitudId = model.TipoSolicitudId,
                ZonaId = model.ZonaId,
                Observaciones = model.Observaciones,
                UsuarioId = userId, // Asignar el GUID del usuario
                FechaCreacion = DateTime.UtcNow,
                Aprobada = false
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.Id }, solicitud);
        }


        [HttpPost("Aprobar/{id}")]
        [Authorize(Roles = "Supervisor,Administrator")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);

            if (solicitud == null)
            {
                return NotFound();
            }

            // Verificar si la solicitud se está respondiendo dentro de las 24 horas
            if (solicitud.FechaCreacion.AddHours(24) < DateTime.UtcNow)
            {
                return BadRequest("La solicitud ha excedido el tiempo límite de respuesta.");
            }

            solicitud.Aprobada = true;
            solicitud.FechaRespuesta = DateTime.UtcNow; // Guardar la fecha de la respuesta

            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }

        [HttpPost("Rechazar/{id}")]
        [Authorize(Roles = "Supervisor,Administrator")]
        public async Task<IActionResult> RechazarSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);

            if (solicitud == null)
            {
                return NotFound();
            }

            // Verificar si la solicitud se está respondiendo dentro de las 24 horas
            if (solicitud.FechaCreacion.AddHours(24) < DateTime.UtcNow)
            {
                return BadRequest("La solicitud ha excedido el tiempo límite de respuesta.");
            }

            solicitud.Aprobada = false;
            solicitud.FechaRespuesta = DateTime.UtcNow; // Guardar la fecha de la respuesta

            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }
    }
}
