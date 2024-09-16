using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoRegistro.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreacionTablaCajeroUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cajero",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FichasCargar = table.Column<int>(type: "int", nullable: false),
                    PorcentajeComision = table.Column<double>(type: "float", nullable: false),
                    UrlImagen = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cajero", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cajeros_DNI",
                table: "Cajero",
                column: "DNI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cajeros_Email",
                table: "Cajero",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cajero");
        }
    }
}
