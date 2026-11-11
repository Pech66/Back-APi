using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LABOFMUTANTAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialPwaSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    UltimaSecuencia = table.Column<long>(type: "bigint", nullable: false),
                    ActualizadoUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotasDiario",
                columns: table => new
                {
                    DispositivoId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    IdLocal = table.Column<long>(type: "bigint", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    CreadaUnixMs = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasDiario", x => new { x.DispositivoId, x.IdLocal });
                    table.ForeignKey(
                        name: "FK_NotasDiario_Dispositivos_DispositivoId",
                        column: x => x.DispositivoId,
                        principalTable: "Dispositivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                columns: table => new
                {
                    DispositivoId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles", x => x.DispositivoId);
                    table.ForeignKey(
                        name: "FK_Perfiles_Dispositivos_DispositivoId",
                        column: x => x.DispositivoId,
                        principalTable: "Dispositivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Preferencias",
                columns: table => new
                {
                    DispositivoId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Linterna = table.Column<bool>(type: "bit", nullable: false),
                    PestanaGuia = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferencias", x => x.DispositivoId);
                    table.ForeignKey(
                        name: "FK_Preferencias_Dispositivos_DispositivoId",
                        column: x => x.DispositivoId,
                        principalTable: "Dispositivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Progresos",
                columns: table => new
                {
                    DispositivoId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Visitas = table.Column<long>(type: "bigint", nullable: false),
                    PrimeraVisita = table.Column<long>(type: "bigint", nullable: true),
                    UltimaVisita = table.Column<long>(type: "bigint", nullable: true),
                    VecesDetectado = table.Column<long>(type: "bigint", nullable: false),
                    MejorSigilo = table.Column<long>(type: "bigint", nullable: false),
                    TeclasProbadasJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progresos", x => x.DispositivoId);
                    table.ForeignKey(
                        name: "FK_Progresos_Dispositivos_DispositivoId",
                        column: x => x.DispositivoId,
                        principalTable: "Dispositivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotasDiario");

            migrationBuilder.DropTable(
                name: "Perfiles");

            migrationBuilder.DropTable(
                name: "Preferencias");

            migrationBuilder.DropTable(
                name: "Progresos");

            migrationBuilder.DropTable(
                name: "Dispositivos");
        }
    }
}
