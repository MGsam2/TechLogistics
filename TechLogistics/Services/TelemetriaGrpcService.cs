using System.Threading.Channels;
using Grpc.Core;
using TechLogistics.Grpc;

namespace TechLogistics.Services;

public class TelemetriaGrpcService : TelemetriaService.TelemetriaServiceBase
{
    private readonly InventarioNotifier notifier;
    private readonly ILogger<TelemetriaGrpcService> logger;

    public TelemetriaGrpcService(
        InventarioNotifier notifier,
        ILogger<TelemetriaGrpcService> logger)
    {
        this.notifier = notifier;
        this.logger = logger;
    }

    public override async Task SuscribirseInventario(
        SuscripcionInventarioRequest request,
        IServerStreamWriter<InventarioActualizacion> responseStream,
        ServerCallContext context)
    {
        var canal =
            Channel.CreateUnbounded<InventarioActualizadoEvent>();

        async Task RecibirActualizacion(
            InventarioActualizadoEvent evento)
        {
            if (request.CentroId == 0 ||
                evento.Centro.Id == request.CentroId)
            {
                await canal.Writer.WriteAsync(evento);
            }
        }

        notifier.InventarioActualizado += RecibirActualizacion;

        logger.LogInformation(
            "Cliente gRPC suscrito al centro {CentroId}.",
            request.CentroId);

        try
        {
            await foreach (
                var evento in canal.Reader.ReadAllAsync(
                    context.CancellationToken))
            {
                var centro = evento.Centro;

                var actualizacion =
                    new InventarioActualizacion
                    {
                        CentroId = centro.Id,
                        NombreCentro = centro.Nombre,
                        InventarioTotal = centro.Inventario,
                        EnLinea = centro.EnLinea,
                        UltimaActualizacion =
                            centro.UltimaActualizacion
                                .ToString("O")
                    };

                await responseStream.WriteAsync(
                    actualizacion);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation(
                "Cliente gRPC desconectado del centro {CentroId}.",
                request.CentroId);
        }
        finally
        {
            notifier.InventarioActualizado -=
                RecibirActualizacion;

            canal.Writer.TryComplete();

            logger.LogInformation(
                "Suscripción gRPC finalizada para el centro {CentroId}.",
                request.CentroId);
        }
    }
}