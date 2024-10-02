using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoRegistro.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreacionTablaCajeroPlataforma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CajeroPlataforma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CajeroUserId = table.Column<int>(type: "int", nullable: false),
                    PlataformaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajeroPlataforma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CajeroPlataforma_Cajero_CajeroUserId",
                        column: x => x.CajeroUserId,
                        principalTable: "Cajero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CajeroPlataforma_Plataforma_PlataformaId",
                        column: x => x.PlataformaId,
                        principalTable: "Plataforma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CajeroPlataforma_CajeroUserId",
                table: "CajeroPlataforma",
                column: "CajeroUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CajeroPlataforma_PlataformaId",
                table: "CajeroPlataforma",
                column: "PlataformaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CajeroPlataforma");
        }
    }
}
