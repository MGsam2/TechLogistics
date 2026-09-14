using TechLogistics.Services;

namespace TechLogistics.Services;

public class InventarioSimulatorService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<InventarioSimulatorService> logger;

    public InventarioSimulatorService(
        IServiceScopeFactory scopeFactory,
        ILogger<InventarioSimulatorService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Simulador de inventario iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope =
                    scopeFactory.CreateScope();

                var inventarioService =
                    scope.ServiceProvider
                        .GetRequiredService<InventarioService>();

                var notifier =
                    scope.ServiceProvider
                        .GetRequiredService<InventarioNotifier>();

                // ==========================================
                // ACTUALIZAR INVENTARIO
                // ==========================================

                //await inventarioService.ActualizarInventarioAsync(-20, 51);

                // ==========================================
                // OBTENER CENTROS ACTUALIZADOS
                // ==========================================

                var centros = await inventarioService.ObtenerCentrosAsync();

                // ==========================================
                // NOTIFICAR CADA CENTRO
                // ==========================================

                foreach (var centro in centros)
                {
                    await notifier.NotificarActualizacion(
                        new InventarioActualizadoEvent(centro));
                }

                logger.LogInformation(
                    "Actualización automática completada y notificada para {Cantidad} centros.",
                    centros.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error al actualizar automáticamente el inventario.");
            }

            // ==========================================
            // ESPERAR 10 SEGUNDOS
            // ==========================================

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                stoppingToken);
        }

        logger.LogInformation(
            "Simulador de inventario detenido.");
    }
}