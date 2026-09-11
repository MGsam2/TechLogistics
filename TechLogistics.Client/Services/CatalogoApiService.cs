using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class CatalogoApiService
{
    private readonly HttpClient httpClient;

    public CatalogoApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Producto>>(
            "api/catalogo")
            ?? new List<Producto>();
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<Producto>(
            $"api/catalogo/{id}");
    }

    public async Task<Producto?> CrearAsync(
        Producto producto)
    {
        var respuesta = await httpClient.PostAsJsonAsync(
            "api/catalogo",
            producto);

        if (!respuesta.IsSuccessStatusCode)
        {
            return null;
        }

        return await respuesta.Content
            .ReadFromJsonAsync<Producto>();
    }

    public async Task<bool> ActualizarAsync(
        int id,
        Producto producto)
    {
        var respuesta = await httpClient.PutAsJsonAsync(
            $"api/catalogo/{id}",
            producto);

        return respuesta.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var respuesta = await httpClient.DeleteAsync(
            $"api/catalogo/{id}");

        return respuesta.IsSuccessStatusCode;
    }
}