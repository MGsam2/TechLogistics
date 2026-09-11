using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class CentroDistribucionApiService
{
    private readonly HttpClient httpClient;

    public CentroDistribucionApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<CentroDistribucion>> ObtenerTodosAsync()
    {
        return await httpClient.GetFromJsonAsync<List<CentroDistribucion>>(
            "api/centrodistribucion")
            ?? new List<CentroDistribucion>();
    }

    public async Task<CentroDistribucion?> ObtenerPorIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<CentroDistribucion>(
            $"api/centrodistribucion/{id}");
    }

    public async Task<CentroDistribucion?> CrearAsync(
        CentroDistribucion centro)
    {
        var respuesta = await httpClient.PostAsJsonAsync(
            "api/centrodistribucion",
            centro);

        if (!respuesta.IsSuccessStatusCode)
        {
            return null;
        }

        return await respuesta.Content
            .ReadFromJsonAsync<CentroDistribucion>();
    }

    public async Task<bool> ActualizarAsync(
        int id,
        CentroDistribucion centro)
    {
        var respuesta = await httpClient.PutAsJsonAsync(
            $"api/centrodistribucion/{id}",
            centro);

        return respuesta.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var respuesta = await httpClient.DeleteAsync(
            $"api/centrodistribucion/{id}");

        return respuesta.IsSuccessStatusCode;
    }
}