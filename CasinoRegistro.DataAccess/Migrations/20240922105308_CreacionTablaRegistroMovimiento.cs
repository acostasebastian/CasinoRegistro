using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoRegistro.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreacionTablaRegistroMovimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistroMovimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CajeroId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FichasCargadas = table.Column<int>(type: "int", nullable: true),
                    PesosEntregados = table.Column<double>(type: "float", nullable: true),
                    PesosDevueltos = table.Column<double>(type: "float", nullable: true),
                    Comision = table.Column<double>(type: "float", nullable: true),
                    DeudaPesosActual = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroMovimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroMovimiento_Cajero_CajeroId",
                        column: x => x.CajeroId,
                        principalTable: "Cajero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroMovimiento_CajeroId",
                table: "RegistroMovimiento",
                column: "CajeroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistroMovimiento");
        }
    }
}
