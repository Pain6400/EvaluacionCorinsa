
// Data/ApplicationDbContext.cs
using EvaluacionApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Zone> Zones { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<ApplicationUserZone> ApplicationUserZones { get; set; }
        public DbSet<ApplicationUserRequestType> ApplicationUserRequestTypes { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurar la clave primaria para ApplicationUserZone
            builder.Entity<ApplicationUserZone>()
                .HasKey(az => new { az.UserId, az.ZoneId });

            builder.Entity<ApplicationUserZone>()
                .HasOne(az => az.User)
                .WithMany(u => u.ApplicationUserZones)
                .HasForeignKey(az => az.UserId);

            builder.Entity<ApplicationUserZone>()
                .HasOne(az => az.Zone)
                .WithMany(z => z.ApplicationUserZones)
                .HasForeignKey(az => az.ZoneId);

            // Configurar la clave primaria para ApplicationUserRequestType
            builder.Entity<ApplicationUserRequestType>()
                .HasKey(art => new { art.UserId, art.RequestTypeId });

            builder.Entity<ApplicationUserRequestType>()
                .HasOne(art => art.User)
                .WithMany(u => u.ApplicationUserRequestTypes)
                .HasForeignKey(art => art.UserId);

            builder.Entity<ApplicationUserRequestType>()
                .HasOne(art => art.RequestType)
                .WithMany(rt => rt.ApplicationUserRequestTypes)
                .HasForeignKey(art => art.RequestTypeId);

            // Configurar Solicitud
            builder.Entity<Solicitud>()
                .HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Solicitud>()
                .HasOne(s => s.Zona)
                .WithMany()
                .HasForeignKey(s => s.ZonaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Solicitud>()
                .HasOne(s => s.TipoSolicitud)
                .WithMany()
                .HasForeignKey(s => s.TipoSolicitudId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
