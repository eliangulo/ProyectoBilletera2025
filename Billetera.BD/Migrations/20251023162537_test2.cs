using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billetera.BD.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Movimiento que representa una transferencia a otra cuenta.");

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Descripcion",
                value: "Compra realizada de un tipo de moneda a usuario o billetera.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Compra de criptomonedas");

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Descripcion",
                value: "Compra realizada de un tipo de moneda a usuario o billetera");
        }
    }
}
