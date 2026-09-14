using Microsoft.EntityFrameworkCore;
using TechLogistics.Data;
using TechLogistics.Models;

namespace TechLogistics.Services;

public class InventarioService
{
    private readonly TechLogisticsDbContext context;
    private readonly ILogger<InventarioService> logger;

    public InventarioService(
        TechLogisticsDbContext context,
        ILogger<InventarioService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<List<CentroDistribucion>> ObtenerCentrosAsync()
    {
        return await context.CentrosDistribucion
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<List<CentroDistribucion>> ActualizarInventarioAsync(
        int minimoCambio = -20,
        int maximoCambio = 50)
    {
        var centros =
            await context.CentrosDistribucion.ToListAsync();

        var inventarios =
            await context.InventariosProductos.ToListAsync();

        var random = new Random();

        foreach (var inventario in inventarios)
        {
            var stockAnterior = inventario.Stock;

            var cambio =
                random.Next(minimoCambio, maximoCambio + 1);

            inventario.Stock += cambio;

            if (inventario.Stock < 0)
            {
                inventario.Stock = 0;
            }

            var stockNuevo = inventario.Stock;

            inventario.UltimaActualizacion =
                DateTime.UtcNow;

            // ====================================================
            // REGISTRAR MOVIMIENTO EN HISTORIAL
            // ====================================================

            var historial = new InventarioHistorial
            {
                ProductoId = inventario.ProductoId,
                CentroDistribucionId =
                    inventario.CentroDistribucionId,
                StockAnterior = stockAnterior,
                StockNuevo = stockNuevo,
                Diferencia = stockNuevo - stockAnterior,
                TipoMovimiento = "SIMULACION",
                FechaMovimiento = DateTime.UtcNow
            };

            context.InventarioHistorial.Add(historial);

            logger.LogDebug(
                "Producto {ProductoId}, Centro {CentroId}: " +
                "stock {StockAnterior} -> {StockNuevo}, cambio {Cambio}",
                inventario.ProductoId,
                inventario.CentroDistribucionId,
                stockAnterior,
                stockNuevo,
                cambio);
        }

        foreach (var centro in centros)
        {
            centro.Inventario =
                inventarios
                    .Where(i =>
                        i.CentroDistribucionId == centro.Id)
                    .Sum(i => i.Stock);

            centro.EnLinea =
                random.Next(0, 10) < 8;

            centro.UltimaActualizacion =
                DateTime.UtcNow;
        }

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Inventario actualizado correctamente: " +
            "{CantidadCentros} centros y " +
            "{CantidadProductos} registros de inventario.",
            centros.Count,
            inventarios.Count);

        return centros;
    }
}