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

    // ==========================================
    // OBTENER CENTROS
    // ==========================================

    public async Task<List<CentroDistribucion>> ObtenerCentrosAsync()
    {
        return await context.CentrosDistribucion
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    // ==========================================
    // ACTUALIZAR INVENTARIO
    // ==========================================

    public async Task<List<CentroDistribucion>> ActualizarInventarioAsync(
        int minimoCambio = -20,
        int maximoCambio = 50)
    {
        var centros = await context.CentrosDistribucion
            .ToListAsync();

        var inventarios = await context.InventariosProductos
            .ToListAsync();

        var random = new Random();

        // ==========================================
        // 1. ACTUALIZAR STOCK DE CADA PRODUCTO
        // ==========================================

        foreach (var inventario in inventarios)
        {
            // Simula entradas y salidas de inventario.
            // Puede aumentar o disminuir el stock.
            var cambio = random.Next(
                minimoCambio,
                maximoCambio + 1);

            inventario.Stock += cambio;

            // El stock nunca puede ser negativo.
            if (inventario.Stock < 0)
            {
                inventario.Stock = 0;
            }

            inventario.UltimaActualizacion = DateTime.UtcNow;

            logger.LogDebug(
                "Producto {ProductoId}, Centro {CentroId}: cambio {Cambio}, stock actual {Stock}",
                inventario.ProductoId,
                inventario.CentroDistribucionId,
                cambio,
                inventario.Stock);
        }

        // ==========================================
        // 2. RECALCULAR INVENTARIO DE CADA CENTRO
        // ==========================================

        foreach (var centro in centros)
        {
            centro.Inventario = inventarios
                .Where(i =>
                    i.CentroDistribucionId == centro.Id)
                .Sum(i => i.Stock);

            // Simulación del estado operativo del centro.
            // 80% aproximadamente en línea.
            centro.EnLinea = random.Next(0, 10) < 8;

            centro.UltimaActualizacion = DateTime.UtcNow;
        }

        // ==========================================
        // 3. GUARDAR CAMBIOS
        // ==========================================

        await context.SaveChangesAsync();

        // ==========================================
        // 4. REGISTRAR ACTUALIZACIÓN
        // ==========================================

        logger.LogInformation(
            "Inventario actualizado correctamente: {CantidadCentros} centros y {CantidadProductos} registros de inventario.",
            centros.Count,
            inventarios.Count);

        return centros;
    }
}