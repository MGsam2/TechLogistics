using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class OfflineSyncService
{
    private readonly IOfflineStorageService storageService;
    private readonly HttpClient httpClient;

    public OfflineSyncService(
        IOfflineStorageService storageService,
        HttpClient httpClient)
    {
        this.storageService = storageService;
        this.httpClient = httpClient;
    }

    public async Task<int> SincronizarAsync()
    {
        var operaciones =
            await storageService.ObtenerOperacionesAsync();

        if (operaciones.Count == 0)
        {
            return 0;
        }

        var sincronizadas = 0;

        foreach (var operacion in operaciones)
        {
            try
            {
                HttpResponseMessage respuesta;

                if (operacion.TipoOperacion == "CREACION")
                {
                    respuesta =
                        await httpClient.PostAsJsonAsync(
                            "api/inventario",
                            new
                            {
                                productoId =
                                    operacion.ProductoId,

                                centroDistribucionId =
                                    operacion.CentroDistribucionId,

                                stock =
                                    operacion.Stock
                            });
                }
                else if (operacion.TipoOperacion == "ELIMINACION")
                {
                    respuesta =
                        await httpClient.DeleteAsync(
                            $"api/inventario/{operacion.InventarioId}");
                }
                else
                {
                    respuesta =
                        await httpClient.PutAsJsonAsync(
                            $"api/inventario/{operacion.InventarioId}",
                            new
                            {
                                id =
                                    operacion.InventarioId,

                                productoId =
                                    operacion.ProductoId,

                                centroDistribucionId =
                                    operacion.CentroDistribucionId,

                                stock =
                                    operacion.Stock
                            });
                }

                if (respuesta.IsSuccessStatusCode)
                {
                    await storageService
                        .EliminarOperacionAsync(
                            operacion.Id);

                    sincronizadas++;
                }
            }
            catch
            {
                // Si todavía no hay conexión,
                // conservar la operación pendiente.
                break;
            }
        }

        return sincronizadas;
    }

    public async Task<int> OperacionesPendientesAsync()
    {
        return await storageService
            .ObtenerCantidadAsync();
    }
}
/*using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class OfflineSyncService
{
    private readonly OfflineStorageService storageService;
    private readonly HttpClient httpClient;

    public OfflineSyncService(
        OfflineStorageService storageService,
        HttpClient httpClient)
    {
        this.storageService = storageService;
        this.httpClient = httpClient;
    }

    public async Task<int> SincronizarAsync()
    {
        var operaciones =
            await storageService.ObtenerOperacionesAsync();

        if (operaciones.Count == 0)
        {
            return 0;
        }

        var sincronizadas = 0;

        foreach (var operacion in operaciones)
        {
            try
            {
                HttpResponseMessage respuesta;

                if (operacion.TipoOperacion == "CREACION")
                {
                    respuesta =
                        await httpClient.PostAsJsonAsync(
                            "api/inventario",
                            new
                            {
                                productoId =
                                    operacion.ProductoId,

                                centroDistribucionId =
                                    operacion.CentroDistribucionId,

                                stock =
                                    operacion.Stock
                            });
                }
                else if (operacion.TipoOperacion == "ELIMINACION")
                {
                    respuesta =
                        await httpClient.DeleteAsync(
                            $"api/inventario/{operacion.InventarioId}");
                }
                else
                {
                    respuesta =
                        await httpClient.PutAsJsonAsync(
                            $"api/inventario/{operacion.InventarioId}",
                            new
                            {
                                id =
                                    operacion.InventarioId,

                                productoId =
                                    operacion.ProductoId,

                                centroDistribucionId =
                                    operacion.CentroDistribucionId,

                                stock =
                                    operacion.Stock
                            });
                }

                if (respuesta.IsSuccessStatusCode)
                {
                    await storageService
                        .EliminarOperacionAsync(
                            operacion.Id);

                    sincronizadas++;
                }
            }
            catch
            {
                // Si todavía no hay conexión,
                // conservar la operación pendiente.
                break;
            }
        }

        return sincronizadas;
    }

    public async Task<int> OperacionesPendientesAsync()
    {
        return await storageService
            .ObtenerCantidadAsync();
    }
}*/