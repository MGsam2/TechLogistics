using Microsoft.JSInterop;

namespace TechLogistics.Client.Services;

public class ConectividadService : IAsyncDisposable
{
    private readonly IJSRuntime jsRuntime;

    private DotNetObjectReference<ConectividadService>? referencia;

    public bool EstaConectado { get; private set; }

    public event Action<bool>? EstadoConexionCambiado;

    public ConectividadService(
        IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task IniciarAsync()
    {
        referencia =
            DotNetObjectReference.Create(this);

        await jsRuntime.InvokeVoidAsync(
            "techLogisticsConnectivity.iniciar",
            referencia);
    }

    [JSInvokable]
    public async Task CambiarEstadoConexion(
        bool conectado)
    {
        var estadoAnterior = EstaConectado;

        EstaConectado = conectado;

        EstadoConexionCambiado?.Invoke(
            EstaConectado);
    }

    public ValueTask DisposeAsync()
    {
        referencia?.Dispose();

        return ValueTask.CompletedTask;
    }
}