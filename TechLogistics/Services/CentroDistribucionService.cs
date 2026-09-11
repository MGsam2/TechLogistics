using TechLogistics.Models;

namespace TechLogistics.Services;

public class CentroDistribucionService
{
    private readonly InventarioService inventarioService;
    private readonly InventarioNotifier notifier;

    public CentroDistribucionService(
        InventarioService inventarioService,
        InventarioNotifier notifier)
    {
        this.inventarioService = inventarioService;
        this.notifier = notifier;
    }

    public async Task<List<CentroDistribucion>> ObtenerCentrosAsync()
    {
        return await inventarioService.ObtenerCentrosAsync();
    }

    public async Task ActualizarInventarioAsync()
    {
        var centrosAntes =
            await inventarioService.ObtenerCentrosAsync();

        await inventarioService.ActualizarInventarioAsync(
            -50,
            101);

        var centrosDespues =
            await inventarioService.ObtenerCentrosAsync();

        foreach (var centro in centrosDespues)
        {
            await notifier.NotificarActualizacion(
                new InventarioActualizadoEvent(centro));
        }
    }
}