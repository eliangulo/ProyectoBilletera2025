using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billetera.BD.Migrations
{
    /// <inheritdoc />
    public partial class Inicio1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Billetera",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Billera_Admin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billetera", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monedas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoMoneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitada = table.Column<bool>(type: "bit", nullable: false),
                    CodISO = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monedas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BilleteraId = table.Column<int>(type: "int", nullable: false),
                    CUIL = table.Column<long>(type: "bigint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domicilio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Billetera_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billetera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposCuentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Moneda_Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonedaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCuentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposCuentas_Monedas_MonedaId",
                        column: x => x.MonedaId,
                        principalTable: "Monedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BilleteraId = table.Column<int>(type: "int", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumCuenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCuentaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuentas_Billetera_BilleteraId",
                        column: x => x.BilleteraId,
                        principalTable: "Billetera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuentas_TiposCuentas_TipoCuentaId",
                        column: x => x.TipoCuentaId,
                        principalTable: "TiposCuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "Cuenta_Billetera_Tipo_UQ",
                table: "Cuentas",
                columns: new[] { "BilleteraId", "TipoCuentaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_TipoCuentaId",
                table: "Cuentas",
                column: "TipoCuentaId");

            migrationBuilder.CreateIndex(
                name: "Moneda_CodISO_UQ",
                table: "Monedas",
                column: "CodISO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposCuentas_MonedaId",
                table: "TiposCuentas",
                column: "MonedaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_BilleteraId",
                table: "Usuario",
                column: "BilleteraId");

            migrationBuilder.CreateIndex(
                name: "Usuarios_Correo_UQ",
                table: "Usuario",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Usuarios_CUIL_UQ",
                table: "Usuario",
                column: "CUIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "TiposCuentas");

            migrationBuilder.DropTable(
                name: "Billetera");

            migrationBuilder.DropTable(
                name: "Monedas");
        }
    }
}
