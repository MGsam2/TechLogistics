using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class InventarioApiService
{
    private readonly HttpClient httpClient;

    public InventarioApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    // ==========================================
    // INVENTARIO
    // ==========================================

    public async Task<List<InventarioProducto>> ObtenerInventarioAsync()
    {
        var respuesta = await httpClient.GetAsync(
            "api/inventario");

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content
            .ReadFromJsonAsync<List<InventarioProducto>>()
            ?? new List<InventarioProducto>();
    }

    public async Task<InventarioProducto?> ObtenerPorIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<InventarioProducto>(
            $"api/inventario/{id}");
    }

    public async Task<List<InventarioProducto>> ObtenerPorProductoAsync(
        int productoId)
    {
        return await httpClient.GetFromJsonAsync<List<InventarioProducto>>(
            $"api/inventario/producto/{productoId}")
            ?? new List<InventarioProducto>();
    }

    public async Task<List<InventarioProducto>> ObtenerPorCentroAsync(
        int centroId)
    {
        return await httpClient.GetFromJsonAsync<List<InventarioProducto>>(
            $"api/inventario/centro/{centroId}")
            ?? new List<InventarioProducto>();
    }

    // ==========================================
    // PRODUCTOS
    // ==========================================

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Producto>>(
            "api/catalogo")
            ?? new List<Producto>();
    }

    // ==========================================
    // CENTROS
    // ==========================================

    public async Task<List<CentroDistribucion>> ObtenerCentrosAsync()
    {
        return await httpClient.GetFromJsonAsync<List<CentroDistribucion>>(
            "api/centrodistribucion")
            ?? new List<CentroDistribucion>();
    }

    // ==========================================
    // CREAR
    // ==========================================

    public async Task<InventarioProducto?> CrearAsync(
        InventarioProducto inventario)
    {
        var respuesta = await httpClient.PostAsJsonAsync(
            "api/inventario",
            inventario);

        if (!respuesta.IsSuccessStatusCode)
        {
            return null;
        }

        return await respuesta.Content
            .ReadFromJsonAsync<InventarioProducto>();
    }

    // ==========================================
    // ACTUALIZAR
    // ==========================================

    public async Task<bool> ActualizarAsync(
        int id,
        InventarioProducto inventario)
    {
        var respuesta = await httpClient.PutAsJsonAsync(
            $"api/inventario/{id}",
            inventario);

        return respuesta.IsSuccessStatusCode;
    }

    // ==========================================
    // ELIMINAR
    // ==========================================

    public async Task<bool> EliminarAsync(int id)
    {
        var respuesta = await httpClient.DeleteAsync(
            $"api/inventario/{id}");

        return respuesta.IsSuccessStatusCode;
    }
}