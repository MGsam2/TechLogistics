using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechLogistics.Migrations
{
    /// <inheritdoc />
    public partial class VerificarCambiosInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventariosProductos_ProductoId",
                table: "InventariosProductos");

            migrationBuilder.CreateIndex(
                name: "IX_InventariosProductos_ProductoId_CentroDistribucionId",
                table: "InventariosProductos",
                columns: new[] { "ProductoId", "CentroDistribucionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventariosProductos_ProductoId_CentroDistribucionId",
                table: "InventariosProductos");

            migrationBuilder.CreateIndex(
                name: "IX_InventariosProductos_ProductoId",
                table: "InventariosProductos",
                column: "ProductoId");
        }
    }
}
