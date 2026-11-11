using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LABOFMUTANTAPI.Migrations
{
    /// <inheritdoc />
    public partial class PublicComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    CreadoUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_AutorId_IdCliente",
                table: "Comentarios",
                columns: new[] { "AutorId", "IdCliente" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");
        }
    }
}
