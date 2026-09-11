using Microsoft.Extensions.Logging;

namespace TechLogistics.Client.Services;

public class InventarioStreamingService
{
    private readonly TelemetriaGrpcClientService grpcClient;
    private readonly InventarioClientState state;
    private readonly ILogger<InventarioStreamingService> logger;

    private CancellationTokenSource? cancellationTokenSource;
    private Task? streamingTask;

    public bool EstaConectado { get; private set; }

    public InventarioStreamingService(
        TelemetriaGrpcClientService grpcClient,
        InventarioClientState state,
        ILogger<InventarioStreamingService> logger)
    {
        this.grpcClient = grpcClient;
        this.state = state;
        this.logger = logger;
    }

    public void Iniciar(int centroId = 0)
    {
        if (streamingTask is not null &&
            !streamingTask.IsCompleted)
        {
            return;
        }

        cancellationTokenSource =
            new CancellationTokenSource();

        streamingTask =
            EscucharAsync(
                centroId,
                cancellationTokenSource.Token);
    }

    public async Task DetenerAsync()
    {
        if (cancellationTokenSource is null)
        {
            return;
        }

        await cancellationTokenSource.CancelAsync();

        if (streamingTask is not null)
        {
            try
            {
                await streamingTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
        streamingTask = null;

        EstaConectado = false;
    }

    private async Task EscucharAsync(
        int centroId,
        CancellationToken cancellationToken)
    {
        try
        {
            EstaConectado = true;

            await foreach (
                var actualizacion
                in grpcClient.SuscribirseInventarioAsync(
                    centroId,
                    cancellationToken))
            {
                state.Actualizar(actualizacion);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation(
                "Streaming de inventario detenido.");
        }
        catch (Exception ex)
        {
            EstaConectado = false;

            logger.LogError(
                ex,
                "Error en el streaming de inventario.");
        }
        finally
        {
            EstaConectado = false;
        }
    }
}