using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Billetera.BD.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Monedas_MonedaId",
                table: "Movimientos");

            migrationBuilder.DropIndex(
                name: "IX_Movimientos_MonedaId",
                table: "Movimientos");

            migrationBuilder.DeleteData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "CantidadMoneda",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "ComisionMonto",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "ComisionPorcentaje",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "MonedaId",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "PrecioUnitario",
                table: "Movimientos");

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Operacion",
                value: "suma");

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Nombre", "Operacion" },
                values: new object[] { "Extraccion", "resta" });

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Nombre", "Operacion" },
                values: new object[] { "Transferencia", "transferencia" });

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "Nombre", "Operacion" },
                values: new object[] { "Compra realizada de un tipo de moneda a usuario o billetera", "Compra", "resta" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CantidadMoneda",
                table: "Movimientos",
                type: "decimal(18,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ComisionMonto",
                table: "Movimientos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ComisionPorcentaje",
                table: "Movimientos",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonedaId",
                table: "Movimientos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioUnitario",
                table: "Movimientos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Operacion",
                value: "Ingreso");

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Nombre", "Operacion" },
                values: new object[] { "Retiro", "Egreso" });

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Nombre", "Operacion" },
                values: new object[] { "Compra Cripto", "Egreso" });

            migrationBuilder.UpdateData(
                table: "TipoMovimientos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "Nombre", "Operacion" },
                values: new object[] { "Venta de criptomonedas", "Venta Cripto", "Ingreso" });

            migrationBuilder.InsertData(
                table: "TipoMovimientos",
                columns: new[] { "Id", "Descripcion", "Nombre", "Operacion" },
                values: new object[,]
                {
                    { 5, "Comisión por operación", "Comisión", "Egreso" },
                    { 6, "Transferencia enviada a otra cuenta", "Transferencia Enviada", "Egreso" },
                    { 7, "Transferencia recibida de otra cuenta", "Transferencia Recibida", "Ingreso" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_MonedaId",
                table: "Movimientos",
                column: "MonedaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Monedas_MonedaId",
                table: "Movimientos",
                column: "MonedaId",
                principalTable: "Monedas",
                principalColumn: "Id");
        }
    }
}
