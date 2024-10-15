using EvaluacionApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EvaluacionApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Zones
            if (!context.Zones.Any())
            {
                context.Zones.AddRange(
                    new Zone { Name = "Norte" },
                    new Zone { Name = "Sur" },
                    new Zone { Name = "Centro" },
                    new Zone { Name = "Este" },
                    new Zone { Name = "Oeste" }
                );

                await context.SaveChangesAsync();
            }

            // Seed RequestTypes
            if (!context.RequestTypes.Any())
            {
                context.RequestTypes.AddRange(
                    new RequestType { Name = "Tipo1" },
                    new RequestType { Name = "Tipo2" },
                    new RequestType { Name = "Tipo3" }
                );

                await context.SaveChangesAsync();
            }

            // Asignar rol de Administrador al usuario "usuario@example.com"
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "usuario@example.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null)
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Administrator"))
                {
                    var result = await userManager.AddToRoleAsync(adminUser, "Administrator");
                    if (!result.Succeeded)
                    {
                        // Manejar errores
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"No se pudo asignar el rol de Administrador al usuario. Errores: {errors}");
                    }
                }
            }
            else
            {
                // Opcional: Crear el usuario si no existe
                var newAdminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nombres = "Admin",
                    Apellidos = "Usuario",
                    FechaNacimiento = DateTime.UtcNow,
                    Genero = "Masculino",
                    TwoFactorEnabled = false // Asigna según sea necesario
                };

                var createResult = await userManager.CreateAsync(newAdminUser, "Admin123!"); // Cambia la contraseña según necesidad
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Administrator");
                }
                else
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"No se pudo crear el usuario administrador. Errores: {errors}");
                }
            }
        }
    }
}
