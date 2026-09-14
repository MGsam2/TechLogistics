using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TechLogistics.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarHistorialInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    CentroDistribucionId = table.Column<int>(type: "integer", nullable: false),
                    StockAnterior = table.Column<int>(type: "integer", nullable: false),
                    StockNuevo = table.Column<int>(type: "integer", nullable: false),
                    Diferencia = table.Column<int>(type: "integer", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioHistorial_CentrosDistribucion_CentroDistribucionId",
                        column: x => x.CentroDistribucionId,
                        principalTable: "CentrosDistribucion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioHistorial_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventarioHistorial_CentroDistribucionId",
                table: "InventarioHistorial",
                column: "CentroDistribucionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioHistorial_FechaMovimiento",
                table: "InventarioHistorial",
                column: "FechaMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioHistorial_ProductoId_CentroDistribucionId",
                table: "InventarioHistorial",
                columns: new[] { "ProductoId", "CentroDistribucionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventarioHistorial");
        }
    }
}
