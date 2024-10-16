using EvaluacionApi.Data;
using EvaluacionApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EvaluacionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator,Supervisor")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene el promedio de atención (tiempo promedio entre creación y respuesta de solicitudes).
        /// </summary>
        /// <returns>Promedio de atención en horas.</returns>
        [HttpGet("AverageResponseTime")]
        public async Task<IActionResult> GetAverageResponseTime()
        {
            try
            {
                var promedio = await _context.Solicitudes
                    .Where(s => s.FechaRespuesta.HasValue)
                    .Select(s => (s.FechaRespuesta.Value - s.FechaCreacion).TotalHours)
                    .AverageAsync();

                return Ok(new { AverageResponseTimeInHours = promedio });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error al generar el reporte.");
            }
        }

        /// <summary>
        /// Obtiene el número de solicitudes resueltas, aprobadas y no aprobadas.
        /// </summary>
        /// <returns>Conteo de solicitudes resueltas.</returns>
        [HttpGet("ResolvedSolicitudes")]
        public async Task<IActionResult> GetResolvedSolicitudes()
        {
            try
            {
                var aprobadas = await _context.Solicitudes
                    .Where(s => s.Aprobada == true)
                    .CountAsync();

                var noAprobadas = await _context.Solicitudes
                    .Where(s => s.Aprobada == false)
                    .CountAsync();

                return Ok(new
                {
                    ResolvedSolicitudes = aprobadas + noAprobadas,
                    Approved = aprobadas,
                    NotApproved = noAprobadas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error al generar el reporte.");
            }
        }

        /// <summary>
        /// Obtiene el número de solicitudes no atendidas (no resueltas dentro de 24 horas).
        /// </summary>
        /// <returns>Conteo de solicitudes no atendidas.</returns>
        [HttpGet("UnattendedSolicitudes")]
        public async Task<IActionResult> GetUnattendedSolicitudes()
        {
            try
            {
                var fechaLimite = DateTime.UtcNow.AddHours(-24);

                var noAtendidas = await _context.Solicitudes
                    .Where(s => !s.FechaRespuesta.HasValue && s.FechaCreacion < fechaLimite)
                    .CountAsync();

                return Ok(new { UnattendedSolicitudes = noAtendidas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error al generar el reporte.");
            }
        }

        /// <summary>
        /// Obtiene solicitudes con filtros específicos.
        /// </summary>
        /// <param name="filters">Filtros para las solicitudes.</param>
        /// <returns>Listado de solicitudes filtradas.</returns>
        [HttpGet("FilteredSolicitudes")]
        public async Task<IActionResult> GetFilteredSolicitudes([FromQuery] SolicitudFiltersViewModel filters)
        {
            try
            {
                var query = _context.Solicitudes.AsQueryable();

                if (filters.StartDate.HasValue)
                {
                    query = query.Where(s => s.FechaCreacion >= filters.StartDate.Value);
                }

                if (filters.EndDate.HasValue)
                {
                    query = query.Where(s => s.FechaCreacion <= filters.EndDate.Value);
                }

                if (!string.IsNullOrEmpty(filters.Status))
                {
                    if (filters.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(s => s.Aprobada == true);
                    }
                    else if (filters.Status.Equals("NotApproved", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.Where(s => s.Aprobada == false);
                    }
                }

                var solicitudes = await query
                    .Include(s => s.Usuario)
                    .ToListAsync();

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error al generar el reporte.");
            }
        }
    }
}
