using EvaluacionApi.Data;
using EvaluacionApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EvaluacionApi.Authorization
{
    public class SupervisorAuthorizationHandler : AuthorizationHandler<SupervisorAuthorizationRequirement, Solicitud>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SupervisorAuthorizationHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SupervisorAuthorizationRequirement requirement, Solicitud resource)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            // Verificar que el usuario tenga el rol de Supervisor
            if (!await _userManager.IsInRoleAsync(user, "Supervisor"))
            {
                return;
            }

            // Verificar que el supervisor tenga asignada la zona y el tipo de solicitud de la solicitud
            var hasZone = await _context.ApplicationUserZones
                .AnyAsync(az => az.UserId == user.Id && az.ZoneId == resource.ZonaId);

            var hasRequestType = await _context.ApplicationUserRequestTypes
                .AnyAsync(art => art.UserId == user.Id && art.RequestTypeId == resource.TipoSolicitudId);

            if (hasZone && hasRequestType)
            {
                context.Succeed(requirement);
            }
        }
    }
}
