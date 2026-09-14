using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class InventarioHistorialApiService
{
    private readonly HttpClient httpClient;

    public InventarioHistorialApiService(
        HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<InventarioHistorialDto>>
        ObtenerHistorialAsync()
    {
        return await httpClient.GetFromJsonAsync<
            List<InventarioHistorialDto>>(
                "api/inventario/historial")
            ?? new List<InventarioHistorialDto>();
    }

    public async Task<InventarioHistorialDto?>
        ObtenerPorIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<
            InventarioHistorialDto>(
                $"api/inventario/historial/{id}");
    }

    public async Task<List<InventarioHistorialDto>>
        ObtenerPorProductoAsync(int productoId)
    {
        return await httpClient.GetFromJsonAsync<
            List<InventarioHistorialDto>>(
                $"api/inventario/historial/producto/{productoId}")
            ?? new List<InventarioHistorialDto>();
    }

    public async Task<List<InventarioHistorialDto>>
        ObtenerPorCentroAsync(int centroId)
    {
        return await httpClient.GetFromJsonAsync<
            List<InventarioHistorialDto>>(
                $"api/inventario/historial/centro/{centroId}")
            ?? new List<InventarioHistorialDto>();
    }
}