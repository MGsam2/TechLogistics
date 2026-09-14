using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public interface IOfflineStorageService
{
    Task<List<OperacionOffline>> ObtenerOperacionesAsync();

    Task GuardarOperacionAsync(
        OperacionOffline operacion);

    Task EliminarOperacionAsync(
        Guid id);

    Task<int> ObtenerCantidadAsync();

    Task GuardarInventarioCacheAsync(
        List<InventarioProducto> inventarios);

    Task<List<InventarioProducto>>
        ObtenerInventarioCacheAsync();

    Task GuardarProductosCacheAsync(
        List<Producto> productos);

    Task<List<Producto>>
        ObtenerProductosCacheAsync();

    Task GuardarCentrosCacheAsync(
        List<CentroDistribucion> centros);

    Task<List<CentroDistribucion>>
        ObtenerCentrosCacheAsync();
}