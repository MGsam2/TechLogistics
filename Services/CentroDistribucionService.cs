using Microsoft.EntityFrameworkCore;
using TechLogistics.Data;
using TechLogistics.Models;

namespace TechLogistics.Services;

public class CentroDistribucionService
{
    private readonly TechLogisticsDbContext context;

    public CentroDistribucionService(TechLogisticsDbContext context)
    {
        this.context = context;
    }

    public async Task<List<CentroDistribucion>> ObtenerCentrosAsync()
    {
        return await context.CentrosDistribucion
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task ActualizarInventarioAsync()
    {
        var centros = await context.CentrosDistribucion.ToListAsync();

        var random = new Random();

        foreach (var centro in centros)
        {
            // Simulación de cambios de inventario
            centro.Inventario += random.Next(-50, 101);

            // Evitar inventarios negativos
            if (centro.Inventario < 0)
            {
                centro.Inventario = 0;
            }

            // Simulación del estado del centro
            centro.EnLinea = random.Next(0, 10) > 1;

            // PostgreSQL/Npgsql trabaja con UTC
            centro.UltimaActualizacion = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }
}