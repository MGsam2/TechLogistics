using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TechLogistics.Controllers;
using TechLogistics.Data;
using TechLogistics.Models;
using Xunit;

namespace TechLogistics.Tests.Controllers;

public class CentroDistribucionControllerTests
{
    private static TechLogisticsDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<TechLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TechLogisticsDbContext(options);
    }

    private static CentroDistribucionController CrearController(
        TechLogisticsDbContext context)
    {
        return new CentroDistribucionController(
            context,
            NullLogger<CentroDistribucionController>.Instance);
    }

    [Fact]
    public async Task ObtenerCentros_DevuelveCentrosOrdenadosPorId()
    {
        await using var context = CrearContexto();

        context.CentrosDistribucion.AddRange(
            new CentroDistribucion
            {
                Id = 2,
                Nombre = "San Salvador",
                Inventario = 100,
                EnLinea = true
            },
            new CentroDistribucion
            {
                Id = 1,
                Nombre = "Guatemala",
                Inventario = 200,
                EnLinea = true
            });

        await context.SaveChangesAsync();

        var controller = CrearController(context);

        var resultado = await controller.ObtenerCentros();

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);

        var centros = Assert.IsAssignableFrom<
            IEnumerable<CentroDistribucion>>(ok.Value);

        Assert.Equal(2, centros.Count());
        Assert.Equal(1, centros.First().Id);
        Assert.Equal(2, centros.Last().Id);
    }

    [Fact]
    public async Task ObtenerCentro_Existente_DevuelveOk()
    {
        await using var context = CrearContexto();

        context.CentrosDistribucion.Add(
            new CentroDistribucion
            {
                Id = 1,
                Nombre = "Guatemala",
                Inventario = 1245,
                EnLinea = true
            });

        await context.SaveChangesAsync();

        var controller = CrearController(context);

        var resultado = await controller.ObtenerCentro(1);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);

        var centro = Assert.IsType<CentroDistribucion>(ok.Value);

        Assert.Equal(1, centro.Id);
        Assert.Equal("Guatemala", centro.Nombre);
        Assert.Equal(1245, centro.Inventario);
    }

    [Fact]
    public async Task ObtenerCentro_Inexistente_DevuelveNotFound()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var resultado = await controller.ObtenerCentro(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(
            resultado.Result);

        Assert.Contains(
            "no existe",
            notFound.Value?.ToString());
    }

    [Fact]
    public async Task CrearCentro_SinNombre_DevuelveBadRequest()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var centro = new CentroDistribucion
        {
            Nombre = "   ",
            Inventario = 100,
            EnLinea = true
        };

        var resultado = await controller.CrearCentro(centro);

        var badRequest = Assert.IsType<BadRequestObjectResult>(
            resultado.Result);

        Assert.Contains(
            "nombre del centro",
            badRequest.Value?.ToString());
    }

    [Fact]
    public async Task CrearCentro_NombreDuplicado_DevuelveConflict()
    {
        await using var context = CrearContexto();

        context.CentrosDistribucion.Add(
            new CentroDistribucion
            {
                Id = 1,
                Nombre = "Guatemala",
                Inventario = 100,
                EnLinea = true
            });

        await context.SaveChangesAsync();

        var controller = CrearController(context);

        var centro = new CentroDistribucion
        {
            Nombre = "guatemala",
            Inventario = 200,
            EnLinea = true
        };

        var resultado = await controller.CrearCentro(centro);

        var conflict = Assert.IsType<ConflictObjectResult>(
            resultado.Result);

        Assert.Contains(
            "Ya existe",
            conflict.Value?.ToString());
    }

    [Fact]
    public async Task CrearCentro_Valido_CreaCentroYDevuelveCreated()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var centro = new CentroDistribucion
        {
            Nombre = "Tegucigalpa",
            Inventario = 431,
            EnLinea = true
        };

        var resultado = await controller.CrearCentro(centro);

        var created = Assert.IsType<CreatedAtActionResult>(
            resultado.Result);

        var centroCreado =
            Assert.IsType<CentroDistribucion>(created.Value);

        Assert.Equal("Tegucigalpa", centroCreado.Nombre);
        Assert.Equal(431, centroCreado.Inventario);
        Assert.NotEqual(default, centroCreado.UltimaActualizacion);

        var centroEnDb =
            await context.CentrosDistribucion.SingleAsync();

        Assert.Equal("Tegucigalpa", centroEnDb.Nombre);
    }

    [Fact]
    public async Task ActualizarCentro_IdDiferente_DevuelveBadRequest()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var centro = new CentroDistribucion
        {
            Id = 2,
            Nombre = "Guatemala",
            Inventario = 100,
            EnLinea = true
        };

        var resultado =
            await controller.ActualizarCentro(1, centro);

        var badRequest = Assert.IsType<BadRequestObjectResult>(
            resultado);

        Assert.Contains(
            "ID de la URL",
            badRequest.Value?.ToString());
    }

    [Fact]
    public async Task ActualizarCentro_SinNombre_DevuelveBadRequest()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var centro = new CentroDistribucion
        {
            Id = 1,
            Nombre = "",
            Inventario = 100,
            EnLinea = true
        };

        var resultado =
            await controller.ActualizarCentro(1, centro);

        var badRequest = Assert.IsType<BadRequestObjectResult>(
            resultado);

        Assert.Contains(
            "nombre del centro",
            badRequest.Value?.ToString());
    }

    [Fact]
    public async Task ActualizarCentro_Inexistente_DevuelveNotFound()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var centro = new CentroDistribucion
        {
            Id = 999,
            Nombre = "Centro Nuevo",
            Inventario = 100,
            EnLinea = true
        };

        var resultado =
            await controller.ActualizarCentro(999, centro);

        var notFound = Assert.IsType<NotFoundObjectResult>(
            resultado);

        Assert.Contains(
            "no existe",
            notFound.Value?.ToString());
    }

    [Fact]
    public async Task ActualizarCentro_NombreDuplicado_DevuelveConflict()
    {
        await using var context = CrearContexto();

        context.CentrosDistribucion.AddRange(
            new CentroDistribucion
            {
                Id = 1,
                Nombre = "Guatemala",
                Inventario = 100,
                EnLinea = true
            },
            new CentroDistribucion
            {
                Id = 2,
                Nombre = "San Salvador",
                Inventario = 200,
                EnLinea = true
            });

        await context.SaveChangesAsync();

        var controller = CrearController(context);

        var centro = new CentroDistribucion
        {
            Id = 2,
            Nombre = "GUATEMALA",
            Inventario = 300,
            EnLinea = false
        };

        var resultado =
            await controller.ActualizarCentro(2, centro);

        var conflict = Assert.IsType<ConflictObjectResult>(
            resultado);

        Assert.Contains(
            "Ya existe otro centro",
            conflict.Value?.ToString());
    }

    [Fact]
    public async Task ActualizarCentro_Valido_ActualizaDatosYDevuelveNoContent()
    {
        await using var context = CrearContexto();

        context.CentrosDistribucion.Add(
            new CentroDistribucion
            {
                Id = 1,
                Nombre = "Guatemala",
                Inventario = 100,
                EnLinea = true
            });

        await context.SaveChangesAsync();

        var controller = CrearController(context);

        var centroActualizado = new CentroDistribucion
        {
            Id = 1,
            Nombre = "Guatemala Central",
            Inventario = 500,
            EnLinea = false
        };

        var resultado =
            await controller.ActualizarCentro(
                1,
                centroActualizado);

        Assert.IsType<NoContentResult>(resultado);

        var centroEnDb =
            await context.CentrosDistribucion.FindAsync(1);

        Assert.NotNull(centroEnDb);
        Assert.Equal(
            "Guatemala Central",
            centroEnDb.Nombre);
        Assert.Equal(500, centroEnDb.Inventario);
        Assert.False(centroEnDb.EnLinea);
        Assert.NotEqual(
            default,
            centroEnDb.UltimaActualizacion);
    }

    [Fact]
    public async Task EliminarCentro_Inexistente_DevuelveNotFound()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var resultado =
            await controller.EliminarCentro(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(
            resultado);

        Assert.Contains(
            "no existe",
            notFound.Value?.ToString());
    }

    [Fact]
    public async Task EliminarCentro_Existente_EliminaYDevuelveNoContent()
    {
        await using var context = CrearContexto();

        context.CentrosDistribucion.Add(
            new CentroDistribucion
            {
                Id = 1,
                Nombre = "Guatemala",
                Inventario = 100,
                EnLinea = true
            });

        await context.SaveChangesAsync();

        var controller = CrearController(context);

        var resultado =
            await controller.EliminarCentro(1);

        Assert.IsType<NoContentResult>(resultado);

        var centro =
            await context.CentrosDistribucion.FindAsync(1);

        Assert.Null(centro);
    }

    [Fact]
    public async Task ObtenerInventarioCentro_CentroInexistente_DevuelveNotFound()
    {
        await using var context = CrearContexto();

        var controller = CrearController(context);

        var resultado =
            await controller.ObtenerInventarioCentro(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(
            resultado.Result);

        Assert.Contains(
            "no existe",
            notFound.Value?.ToString());
    }

    [Fact]
    public async Task ObtenerInventarioCentro_CentroExistente_DevuelveInventario()
    {
        await using var context = CrearContexto();

        var centro = new CentroDistribucion
        {
            Id = 1,
            Nombre = "Guatemala",
            Inventario = 100,
            EnLinea = true
        };

        var producto1 = new Producto
        {
            Id = 1,
            Nombre = "Producto A",
            Categoria = "General",
            StockMinimo = 10,
            Activo = true
        };

        var producto2 = new Producto
        {
            Id = 2,
            Nombre = "Producto B",
            Categoria = "General",
            StockMinimo = 20,
            Activo = true
        };

        context.CentrosDistribucion.Add(centro);
        context.Productos.AddRange(producto1, producto2);

        context.InventariosProductos.AddRange(
            new InventarioProducto
            {
                Id = 2,
                ProductoId = 2,
                CentroDistribucionId = 1,
                Stock = 50,
                UltimaActualizacion = DateTime.UtcNow
            },
            new InventarioProducto
            {
                Id = 1,
                ProductoId = 1,
                CentroDistribucionId = 1,
                Stock = 100,
                UltimaActualizacion = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var controller = CrearController(context);

        var resultado =
            await controller.ObtenerInventarioCentro(1);

        var ok = Assert.IsType<OkObjectResult>(
            resultado.Result);

        var inventario =
            Assert.IsAssignableFrom<IEnumerable<object>>(
                ok.Value);

        Assert.Equal(2, inventario.Count());
    }
}