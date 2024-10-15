using Microsoft.AspNetCore.Mvc;
using EvaluacionApi.Data;
using EvaluacionApi.Authorization;
using EvaluacionApi.Models;
using EvaluacionApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EvaluacionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación
    public class SolicitudesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public SolicitudesController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
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

            var solicitud = new Solicitud
            {
                TipoSolicitudId = model.TipoSolicitudId,
                ZonaId = model.ZonaId,
                Observaciones = model.Observaciones,
                UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                FechaCreacion = DateTime.UtcNow,
                Aprobada = false
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.Id }, solicitud);
        }

        // POST: api/Solicitudes/Approve/5
        [HttpPost("Approve/{id}")]
        public async Task<IActionResult> ApproveSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            // Autorizar al usuario como Supervisor con privilegios en la zona y tipo de solicitud de esta solicitud
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, solicitud, "SupervisorPolicy");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            // Aprobar la solicitud
            solicitud.Aprobada = true;
            _context.Solicitudes.Update(solicitud);
            await _context.SaveChangesAsync();

            return Ok("Solicitud aprobada.");
        }

        // POST: api/Solicitudes/Reject/5
        [HttpPost("Reject/{id}")]
        public async Task<IActionResult> RejectSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            // Autorizar al usuario como Supervisor con privilegios en la zona y tipo de solicitud de esta solicitud
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, solicitud, "SupervisorPolicy");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            // Rechazar la solicitud
            solicitud.Aprobada = false;
            _context.Solicitudes.Update(solicitud);
            await _context.SaveChangesAsync();

            return Ok("Solicitud rechazada.");
        }
    }
}
