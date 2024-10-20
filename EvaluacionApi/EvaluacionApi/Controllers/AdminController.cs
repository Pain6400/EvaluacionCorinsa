using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EvaluacionApi.Data;
using EvaluacionApi.Models;
using EvaluacionApi.ViewModels;

namespace EvaluacionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Administrator")] // Solo Administradores pueden acceder
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        // POST: api/Admin/AssignSupervisor
        [HttpPost("AssignSupervisor")]
        public async Task<IActionResult> AssignSupervisor([FromBody] AssignSupervisorViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supervisor = await _userManager.FindByEmailAsync(model.SupervisorEmail);
            if (supervisor == null)
                return NotFound("Supervisor no encontrado.");

            // Verificar si el usuario ya es Supervisor, si no, asignar el rol
            if (!await _userManager.IsInRoleAsync(supervisor, "Supervisor"))
            {
                var roleResult = await _userManager.AddToRoleAsync(supervisor, "Supervisor");
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }

            // Asignar Zonas
            var existingUserZones = await _context.ApplicationUserZones
                .Where(az => az.UserId == supervisor.Id)
                .ToListAsync();

            // Eliminar asignaciones existentes
            _context.ApplicationUserZones.RemoveRange(existingUserZones);

            // Asignar nuevas Zonas
            var newUserZones = model.ZoneIds.Select(zoneId => new ApplicationUserZone
            {
                UserId = supervisor.Id,
                ZoneId = zoneId
            });

            await _context.ApplicationUserZones.AddRangeAsync(newUserZones);

            // Asignar Tipos de Solicitud
            var existingUserRequestTypes = await _context.ApplicationUserRequestTypes
                .Where(art => art.UserId == supervisor.Id)
                .ToListAsync();

            _context.ApplicationUserRequestTypes.RemoveRange(existingUserRequestTypes);

            var newUserRequestTypes = model.RequestTypeIds.Select(rtId => new ApplicationUserRequestType
            {
                UserId = supervisor.Id,
                RequestTypeId = rtId
            });

            await _context.ApplicationUserRequestTypes.AddRangeAsync(newUserRequestTypes);

            // Guardar cambios
            await _context.SaveChangesAsync();

            return Ok("Supervisor asignado correctamente a las zonas y tipos de solicitud.");
        }

        // POST: api/Admin/AssignRole
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Buscar al usuario por email
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return NotFound("Usuario no encontrado.");

                var role = await _roleManager.FindByIdAsync(model.Role);
                if (role == null)
                    return NotFound("El rol especificado no existe.");

                // Verificar si el usuario ya tiene el rol asignado
                var roleExists = await _userManager.IsInRoleAsync(user, role.Name);
                if (roleExists)
                    return BadRequest("El usuario ya tiene asignado este rol.");

                // Asignar el rol al usuario por su nombre
                var result = await _userManager.AddToRoleAsync(user, role.Name);
                if (result.Succeeded)
                    return Ok("Rol asignado correctamente.");

                // Manejar los errores de la operación
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción no manejada
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
