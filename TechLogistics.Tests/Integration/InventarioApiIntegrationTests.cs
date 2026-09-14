using System.Net;
using System.Net.Http.Json;
using TechLogistics.Models;
using Xunit;

namespace TechLogistics.Tests.Integration;

public class InventarioApiIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public InventarioApiIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task ObtenerInventario_SinDatosDevuelveOk()
    {
        var response =
            await client.GetAsync("/api/Inventario");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task ObtenerInventarioPorId_InexistenteDevuelveNotFound()
    {
        var response =
            await client.GetAsync(
                "/api/Inventario/99999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CrearInventario_SinProductoDevuelveBadRequest()
    {
        var inventario = new InventarioProducto
        {
            ProductoId = 999,
            CentroDistribucionId = 1,
            Stock = 10
        };

        var response =
            await client.PostAsJsonAsync(
                "/api/Inventario",
                inventario);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task CrearInventario_ConStockNegativoDevuelveBadRequest()
    {
        var inventario = new InventarioProducto
        {
            ProductoId = 1,
            CentroDistribucionId = 1,
            Stock = -10
        };

        var response =
            await client.PostAsJsonAsync(
                "/api/Inventario",
                inventario);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task ObtenerHistorial_DevuelveOk()
    {
        var response =
            await client.GetAsync(
                "/api/Inventario/historial");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}