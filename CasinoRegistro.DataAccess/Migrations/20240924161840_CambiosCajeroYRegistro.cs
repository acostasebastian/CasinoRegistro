using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoRegistro.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CambiosCajeroYRegistro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeudaPesosActual",
                table: "RegistroMovimiento");

            migrationBuilder.AlterColumn<decimal>(
                name: "PesosEntregados",
                table: "RegistroMovimiento",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PesosDevueltos",
                table: "RegistroMovimiento",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Comision",
                table: "RegistroMovimiento",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsIngresoFichas",
                table: "RegistroMovimiento",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DeudaPesosActual",
                table: "Cajero",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsIngresoFichas",
                table: "RegistroMovimiento");

            migrationBuilder.DropColumn(
                name: "DeudaPesosActual",
                table: "Cajero");

            migrationBuilder.AlterColumn<double>(
                name: "PesosEntregados",
                table: "RegistroMovimiento",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PesosDevueltos",
                table: "RegistroMovimiento",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Comision",
                table: "RegistroMovimiento",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DeudaPesosActual",
                table: "RegistroMovimiento",
                type: "float",
                nullable: true);
        }
    }
}
