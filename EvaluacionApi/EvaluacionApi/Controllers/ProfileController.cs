using EvaluacionApi.Models;
using EvaluacionApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvaluacionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Asegura que solo usuarios autenticados accedan
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/Profile
        [HttpGet]
        [Authorize(Roles = "Supervisor,Administrator")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized("Usuario no autenticado.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var profile = new UserProfileViewModel
                {
                    Nombres = user.Nombres,
                    Apellidos = user.Apellidos,
                    FechaNacimiento = user.FechaNacimiento,
                    Genero = user.Genero
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error al obtener el perfil.");
            }
        }

        // PUT: api/Profile
        [HttpPut]
        [Authorize(Roles = "Supervisor,Administrator")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized("Usuario no autenticado.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                // Actualizar propiedades
                user.Nombres = model.Nombres;
                user.Apellidos = model.Apellidos;
                user.FechaNacimiento = model.FechaNacimiento.ToUniversalTime();
                user.Genero = model.Genero;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(user); 
                }

                // Manejar errores de actualización
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error al actualizar el perfil." + ex.Message);
            }
        }
    }
}
