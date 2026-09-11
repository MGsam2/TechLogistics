using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using TechLogistics.Grpc;

namespace TechLogistics.Client.Services;

public class TelemetriaGrpcClientService
{
    private readonly GrpcChannel channel;
    private readonly TelemetriaService.TelemetriaServiceClient client;

    public TelemetriaGrpcClientService()
    {
        var httpHandler = new GrpcWebHandler(
            GrpcWebMode.GrpcWebText,
            new HttpClientHandler());

        channel = GrpcChannel.ForAddress(
            "http://localhost:5134",
            new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });

        client = new TelemetriaService.TelemetriaServiceClient(
            channel);
    }

    public async IAsyncEnumerable<InventarioActualizacion>
        SuscribirseInventarioAsync(
            int centroId = 0,
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken = default)
    {
        using var llamada =
            client.SuscribirseInventario(
                new SuscripcionInventarioRequest
                {
                    CentroId = centroId
                },
                cancellationToken: cancellationToken);

        while (await llamada.ResponseStream.MoveNext(
            cancellationToken))
        {
            yield return llamada.ResponseStream.Current;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await channel.ShutdownAsync();
    }
}