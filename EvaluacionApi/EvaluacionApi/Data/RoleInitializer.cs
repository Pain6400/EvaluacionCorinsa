using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EvaluacionApi.Data
{
    public static class RoleInitializer
    {
        private static readonly string[] Roles = new[] { "Administrator", "Supervisor", "User" };

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Roles)
            {
                var exists = await roleManager.RoleExistsAsync(role);
                if (!exists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
