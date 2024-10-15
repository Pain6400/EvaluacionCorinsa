using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EvaluacionApi.Migrations
{
    /// <inheritdoc />
    public partial class Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_Usuarios_UsuarioId",
                table: "Solicitudes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "Zona",
                table: "Solicitudes");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Solicitudes",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TipoSolicitudId",
                table: "Solicitudes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ZonaId",
                table: "Solicitudes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RequestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserRequestTypes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RequestTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserRequestTypes", x => new { x.UserId, x.RequestTypeId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserRequestTypes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserRequestTypes_RequestTypes_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "RequestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserZones",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ZoneId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserZones", x => new { x.UserId, x.ZoneId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserZones_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserZones_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_TipoSolicitudId",
                table: "Solicitudes",
                column: "TipoSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_ZonaId",
                table: "Solicitudes",
                column: "ZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRequestTypes_RequestTypeId",
                table: "ApplicationUserRequestTypes",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserZones_ZoneId",
                table: "ApplicationUserZones",
                column: "ZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_AspNetUsers_UsuarioId",
                table: "Solicitudes",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_RequestTypes_TipoSolicitudId",
                table: "Solicitudes",
                column: "TipoSolicitudId",
                principalTable: "RequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_Zones_ZonaId",
                table: "Solicitudes",
                column: "ZonaId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_AspNetUsers_UsuarioId",
                table: "Solicitudes");

            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_RequestTypes_TipoSolicitudId",
                table: "Solicitudes");

            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_Zones_ZonaId",
                table: "Solicitudes");

            migrationBuilder.DropTable(
                name: "ApplicationUserRequestTypes");

            migrationBuilder.DropTable(
                name: "ApplicationUserZones");

            migrationBuilder.DropTable(
                name: "RequestTypes");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Solicitudes_TipoSolicitudId",
                table: "Solicitudes");

            migrationBuilder.DropIndex(
                name: "IX_Solicitudes_ZonaId",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "TipoSolicitudId",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "ZonaId",
                table: "Solicitudes");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Solicitudes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Solicitudes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Zona",
                table: "Solicitudes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Apellidos = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_Usuarios_UsuarioId",
                table: "Solicitudes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
