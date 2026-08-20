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
        int maximoCambio = 51)
    {
        var centros = await context.CentrosDistribucion.ToListAsync();

        var random = new Random();

        foreach (var centro in centros)
        {
            var InventarioAnterior = centro.Inventario;
            
            centro.Inventario += random.Next(
                minimoCambio,
                maximoCambio);

            if (centro.Inventario < 0)
            {
                centro.Inventario = 0;
            }

            centro.EnLinea = random.Next(0, 10) > 1;

            centro.UltimaActualizacion = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Inventario actualizado para {Cantidad} centros.",
            centros.Count);

        return centros;
    }
}