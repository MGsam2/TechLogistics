extern alias Client;

using System.Net;
using System.Net.Http;

using TechLogistics.Models;

using ClientModels =
    Client::TechLogistics.Client.Models;

using ClientServices =
    Client::TechLogistics.Client.Services;

using OperacionOffline =
    Client::TechLogistics.Client.Models.OperacionOffline;

using IOfflineStorageService =
    Client::TechLogistics.Client.Services.IOfflineStorageService;

using OfflineSyncService =
    Client::TechLogistics.Client.Services.OfflineSyncService;

namespace TechLogistics.Tests.Services;

public class OfflineSyncServiceTests
{
    [Fact]
    public async Task SincronizarAsync_SinOperacionesDevuelveCero()
    {
        // Arrange
        var storage =
            new FakeOfflineStorageService();

        var handler =
            new FakeHttpMessageHandler();

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("http://localhost/")
            };

        var service =
            new OfflineSyncService(
                storage,
                httpClient);

        // Act
        var resultado =
            await service.SincronizarAsync();

        // Assert
        Assert.Equal(0, resultado);
        Assert.Empty(storage.Operaciones);
    }


    [Fact]
    public async Task SincronizarAsync_CreacionExitosa_EliminaOperacion()
    {
        // Arrange
        var operacion =
            CrearOperacion("CREACION");

        var storage =
            new FakeOfflineStorageService(
                operacion);

        var handler =
            new FakeHttpMessageHandler(
                HttpStatusCode.Created);

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("http://localhost/")
            };

        var service =
            new OfflineSyncService(
                storage,
                httpClient);

        // Act
        var resultado =
            await service.SincronizarAsync();

        // Assert
        Assert.Equal(1, resultado);
        Assert.Empty(storage.Operaciones);

        Assert.NotNull(
            handler.UltimaSolicitud);

        Assert.Equal(
            HttpMethod.Post,
            handler.UltimaSolicitud!.Method);

        Assert.Equal(
            "http://localhost/api/inventario",
            handler.UltimaSolicitud.RequestUri!
                .ToString());
    }


    [Fact]
    public async Task SincronizarAsync_ActualizacionExitosa_EliminaOperacion()
    {
        // Arrange
        var operacion =
            CrearOperacion("ACTUALIZACION");

        var storage =
            new FakeOfflineStorageService(
                operacion);

        var handler =
            new FakeHttpMessageHandler(
                HttpStatusCode.NoContent);

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("http://localhost/")
            };

        var service =
            new OfflineSyncService(
                storage,
                httpClient);

        // Act
        var resultado =
            await service.SincronizarAsync();

        // Assert
        Assert.Equal(1, resultado);
        Assert.Empty(storage.Operaciones);

        Assert.Equal(
            HttpMethod.Put,
            handler.UltimaSolicitud!.Method);

        Assert.Equal(
            $"http://localhost/api/inventario/{operacion.InventarioId}",
            handler.UltimaSolicitud.RequestUri!
                .ToString());
    }


    [Fact]
    public async Task SincronizarAsync_EliminacionExitosa_EliminaOperacion()
    {
        // Arrange
        var operacion =
            CrearOperacion("ELIMINACION");

        var storage =
            new FakeOfflineStorageService(
                operacion);

        var handler =
            new FakeHttpMessageHandler(
                HttpStatusCode.NoContent);

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("http://localhost/")
            };

        var service =
            new OfflineSyncService(
                storage,
                httpClient);

        // Act
        var resultado =
            await service.SincronizarAsync();

        // Assert
        Assert.Equal(1, resultado);
        Assert.Empty(storage.Operaciones);

        Assert.Equal(
            HttpMethod.Delete,
            handler.UltimaSolicitud!.Method);

        Assert.Equal(
            $"http://localhost/api/inventario/{operacion.InventarioId}",
            handler.UltimaSolicitud.RequestUri!
                .ToString());
    }


    [Fact]
    public async Task SincronizarAsync_RespuestaFallida_ConservaOperacion()
    {
        // Arrange
        var operacion =
            CrearOperacion("ACTUALIZACION");

        var storage =
            new FakeOfflineStorageService(
                operacion);

        var handler =
            new FakeHttpMessageHandler(
                HttpStatusCode.BadRequest);

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("http://localhost/")
            };

        var service =
            new OfflineSyncService(
                storage,
                httpClient);

        // Act
        var resultado =
            await service.SincronizarAsync();

        // Assert
        Assert.Equal(0, resultado);
        Assert.Single(storage.Operaciones);
    }


    [Fact]
    public async Task SincronizarAsync_ErrorDeConexion_ConservaOperacion()
    {
        // Arrange
        var operacion =
            CrearOperacion("ACTUALIZACION");

        var storage =
            new FakeOfflineStorageService(
                operacion);

        var handler =
            new FakeHttpMessageHandler(
                lanzarExcepcion: true);

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("http://localhost/")
            };

        var service =
            new OfflineSyncService(
                storage,
                httpClient);

        // Act
        var resultado =
            await service.SincronizarAsync();

        // Assert
        Assert.Equal(0, resultado);
        Assert.Single(storage.Operaciones);
    }


    [Fact]
    public async Task OperacionesPendientesAsync_DevuelveCantidadCorrecta()
    {
        // Arrange
        var storage =
            new FakeOfflineStorageService(
                CrearOperacion("ACTUALIZACION"),
                CrearOperacion("CREACION"));

        var handler =
            new FakeHttpMessageHandler();

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress =
                    new Uri("http://localhost/")
            };

        var service =
            new OfflineSyncService(
                storage,
                httpClient);

        // Act
        var resultado =
            await service.OperacionesPendientesAsync();

        // Assert
        Assert.Equal(2, resultado);
    }


    private static OperacionOffline CrearOperacion(
        string tipo)
    {
        return new OperacionOffline
        {
            Id = Guid.NewGuid(),
            InventarioId = 10,
            ProductoId = 20,
            CentroDistribucionId = 1,
            Stock = 50,
            TipoOperacion = tipo,
            FechaCreacion = DateTime.UtcNow
        };
    }


    private sealed class FakeOfflineStorageService
        : IOfflineStorageService
    {
        public List<OperacionOffline> Operaciones { get; }

        public FakeOfflineStorageService(
            params OperacionOffline[] operaciones)
        {
            Operaciones =
                operaciones.ToList();
        }

        public Task<List<OperacionOffline>>
            ObtenerOperacionesAsync()
        {
            return Task.FromResult(
                Operaciones.ToList());
        }

        public Task GuardarOperacionAsync(
            OperacionOffline operacion)
        {
            Operaciones.Add(operacion);

            return Task.CompletedTask;
        }

        public Task EliminarOperacionAsync(
            Guid id)
        {
            Operaciones.RemoveAll(
                o => o.Id == id);

            return Task.CompletedTask;
        }

        public Task<int> ObtenerCantidadAsync()
        {
            return Task.FromResult(
                Operaciones.Count);
        }

        public Task GuardarInventarioCacheAsync(
            List<ClientModels.InventarioProducto>
                inventarios)
        {
            return Task.CompletedTask;
        }

        public Task<List<ClientModels.InventarioProducto>>
            ObtenerInventarioCacheAsync()
        {
            return Task.FromResult(
                new List<ClientModels.InventarioProducto>());
        }

        public Task GuardarProductosCacheAsync(
            List<ClientModels.Producto>
                productos)
        {
            return Task.CompletedTask;
        }

        public Task<List<ClientModels.Producto>>
            ObtenerProductosCacheAsync()
        {
            return Task.FromResult(
                new List<ClientModels.Producto>());
        }

        public Task GuardarCentrosCacheAsync(
            List<ClientModels.CentroDistribucion>
                centros)
        {
            return Task.CompletedTask;
        }

        public Task<List<ClientModels.CentroDistribucion>>
            ObtenerCentrosCacheAsync()
        {
            return Task.FromResult(
                new List<ClientModels.CentroDistribucion>());
        }
    }


    private sealed class FakeHttpMessageHandler
        : HttpMessageHandler
    {
        private readonly HttpStatusCode statusCode;
        private readonly bool lanzarExcepcion;

        public HttpRequestMessage? UltimaSolicitud
        {
            get;
            private set;
        }

        public FakeHttpMessageHandler(
            HttpStatusCode statusCode =
                HttpStatusCode.OK,
            bool lanzarExcepcion = false)
        {
            this.statusCode = statusCode;
            this.lanzarExcepcion = lanzarExcepcion;
        }

        protected override Task<HttpResponseMessage>
            SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
        {
            UltimaSolicitud = request;

            if (lanzarExcepcion)
            {
                throw new HttpRequestException(
                    "Simulación de pérdida de conexión.");
            }

            return Task.FromResult(
                new HttpResponseMessage(statusCode));
        }
    }
}