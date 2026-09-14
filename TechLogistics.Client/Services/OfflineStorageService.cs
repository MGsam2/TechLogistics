using System.Text.Json;
using Microsoft.JSInterop;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class OfflineStorageService : IOfflineStorageService
{
    private const string StorageKey =
        "techlogistics_operaciones_offline";

    private const string InventarioCacheKey =
        "techlogistics_cache_inventario";

    private const string ProductosCacheKey =
        "techlogistics_cache_productos";

    private const string CentrosCacheKey =
        "techlogistics_cache_centros";

    private readonly IJSRuntime jsRuntime;

    public OfflineStorageService(
        IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    // =========================================================
    // OPERACIONES OFFLINE
    // =========================================================

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

    // =========================================================
    // CACHÉ OFFLINE DE INVENTARIO
    // =========================================================

    public async Task GuardarInventarioCacheAsync(
        List<InventarioProducto> inventarios)
    {
        await GuardarCacheAsync(
            InventarioCacheKey,
            inventarios);
    }

    public async Task<List<InventarioProducto>>
        ObtenerInventarioCacheAsync()
    {
        return await ObtenerCacheAsync<
            List<InventarioProducto>>(
                InventarioCacheKey)
            ?? new List<InventarioProducto>();
    }

    // =========================================================
    // CACHÉ OFFLINE DE PRODUCTOS
    // =========================================================

    public async Task GuardarProductosCacheAsync(
        List<Producto> productos)
    {
        await GuardarCacheAsync(
            ProductosCacheKey,
            productos);
    }

    public async Task<List<Producto>>
        ObtenerProductosCacheAsync()
    {
        return await ObtenerCacheAsync<
            List<Producto>>(
                ProductosCacheKey)
            ?? new List<Producto>();
    }

    // =========================================================
    // CACHÉ OFFLINE DE CENTROS
    // =========================================================

    public async Task GuardarCentrosCacheAsync(
        List<CentroDistribucion> centros)
    {
        await GuardarCacheAsync(
            CentrosCacheKey,
            centros);
    }

    public async Task<List<CentroDistribucion>>
        ObtenerCentrosCacheAsync()
    {
        return await ObtenerCacheAsync<
            List<CentroDistribucion>>(
                CentrosCacheKey)
            ?? new List<CentroDistribucion>();
    }

    // =========================================================
    // MÉTODOS GENERALES DE CACHÉ
    // =========================================================

    private async Task GuardarCacheAsync<T>(
        string key,
        T datos)
    {
        var json =
            JsonSerializer.Serialize(datos);

        await jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            key,
            json);
    }

    private async Task<T?> ObtenerCacheAsync<T>(
        string key)
    {
        var json =
            await jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                key);

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }
}