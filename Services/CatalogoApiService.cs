using System.Net.Http.Json;
using TechLogistics.Models;

namespace TechLogistics.Services;

public class CatalogoService
{
    private readonly HttpClient httpClient;

    public CatalogoService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Producto>>(
            "api/catalogo") ?? new List<Producto>();
    }

    public async Task<Producto?> ObtenerProductoAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<Producto>(
            $"api/catalogo/{id}");
    }

    public async Task<Producto?> CrearProductoAsync(Producto producto)
    {
        var respuesta = await httpClient.PostAsJsonAsync(
            "api/catalogo",
            producto);

        if (!respuesta.IsSuccessStatusCode)
            return null;

        return await respuesta.Content.ReadFromJsonAsync<Producto>();
    }

    public async Task<bool> ActualizarProductoAsync(
        int id,
        Producto producto)
    {
        var respuesta = await httpClient.PutAsJsonAsync(
            $"api/catalogo/{id}",
            producto);

        return respuesta.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarProductoAsync(int id)
    {
        var respuesta = await httpClient.DeleteAsync(
            $"api/catalogo/{id}");

        return respuesta.IsSuccessStatusCode;
    }
}