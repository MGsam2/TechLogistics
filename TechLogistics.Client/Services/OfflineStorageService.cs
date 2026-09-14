using System.Text.Json;
using Microsoft.JSInterop;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class OfflineStorageService
{
    private const string StorageKey =
        "techlogistics_operaciones_offline";

    private readonly IJSRuntime jsRuntime;

    public OfflineStorageService(
        IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task<List<OperacionOffline>>
        ObtenerOperacionesAsync()
    {
        var json =
            await jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                StorageKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<OperacionOffline>();
        }

        return JsonSerializer.Deserialize<
            List<OperacionOffline>>(json)
            ?? new List<OperacionOffline>();
    }

    public async Task GuardarOperacionAsync(
        OperacionOffline operacion)
    {
        var operaciones =
            await ObtenerOperacionesAsync();

        operaciones.Add(operacion);

        await GuardarListaAsync(operaciones);
    }

    public async Task EliminarOperacionAsync(
        Guid id)
    {
        var operaciones =
            await ObtenerOperacionesAsync();

        operaciones.RemoveAll(
            o => o.Id == id);

        await GuardarListaAsync(operaciones);
    }

    public async Task<int> ObtenerCantidadAsync()
    {
        var operaciones =
            await ObtenerOperacionesAsync();

        return operaciones.Count;
    }

    private async Task GuardarListaAsync(
        List<OperacionOffline> operaciones)
    {
        var json =
            JsonSerializer.Serialize(operaciones);

        await jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            StorageKey,
            json);
    }
}