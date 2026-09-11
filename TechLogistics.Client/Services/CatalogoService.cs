using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class CatalogoService
{
    private readonly HttpClient http;

    public CatalogoService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        return await http.GetFromJsonAsync<List<Producto>>(
            "api/catalogo") ?? new List<Producto>();
    }

    public async Task<Producto?> ObtenerProductoAsync(int id)
    {
        return await http.GetFromJsonAsync<Producto>(
            $"api/catalogo/{id}");
    }

    public async Task<bool> CrearProductoAsync(Producto producto)
    {
        var respuesta = await http.PostAsJsonAsync(
            "api/catalogo",
            producto);

        return respuesta.IsSuccessStatusCode;
    }

    public async Task<bool> ActualizarProductoAsync(Producto producto)
    {
        var respuesta = await http.PutAsJsonAsync(
            $"api/catalogo/{producto.Id}",
            producto);

        return respuesta.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarProductoAsync(int id)
    {
        var respuesta = await http.DeleteAsync(
            $"api/catalogo/{id}");

        return respuesta.IsSuccessStatusCode;
    }
}