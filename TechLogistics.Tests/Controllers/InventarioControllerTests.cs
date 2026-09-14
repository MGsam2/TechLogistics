using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TechLogistics.Controllers;
using TechLogistics.Data;
using TechLogistics.Models;

namespace TechLogistics.Tests;

public class InventarioControllerTests
{
    [Fact]
    public async Task ObtenerInventario_ConDatosDevuelveOk()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        // Act
        var resultado =
            await controller.ObtenerInventario();

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(resultado.Result);

        var datos =
            Assert.IsAssignableFrom<IEnumerable<object>>(
                okResult.Value);

        Assert.Single(datos);
    }


    [Fact]
    public async Task ObtenerInventarioPorId_CuandoExisteDevuelveOk()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        // Act
        var resultado =
            await controller.ObtenerInventarioPorId(1);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(resultado.Result);

        Assert.NotNull(okResult.Value);
    }


    [Fact]
    public async Task ObtenerInventarioPorId_CuandoNoExisteDevuelveNotFound()
    {
        // Arrange
        await using var context = CrearContexto();

        var controller = CrearController(context);

        // Act
        var resultado =
            await controller.ObtenerInventarioPorId(999);

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                resultado.Result);

        Assert.Equal(
            "El registro de inventario no existe.",
            notFoundResult.Value);
    }


    [Fact]
    public async Task CrearInventario_ConDatosValidos_CreaInventarioYHistorial()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        var nuevoInventario = new InventarioProducto
        {
            ProductoId = 2,
            CentroDistribucionId = 1,
            Stock = 30
        };

        // Act
        var resultado =
            await controller.CrearInventario(
                nuevoInventario);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(
                resultado.Result);

        Assert.NotNull(createdResult.Value);

        var inventarioGuardado =
            await context.InventariosProductos
                .SingleAsync(i =>
                    i.ProductoId == 2 &&
                    i.CentroDistribucionId == 1);

        Assert.Equal(
            30,
            inventarioGuardado.Stock);

        var historial =
            await context.InventarioHistorial
                .SingleAsync();

        Assert.Equal(
            "CREACION",
            historial.TipoMovimiento);

        Assert.Equal(
            30,
            historial.StockNuevo);

        Assert.Equal(
            30,
            historial.Diferencia);
    }


    [Fact]
    public async Task CrearInventario_DuplicadoDevuelveConflict()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        var inventarioDuplicado = new InventarioProducto
        {
            ProductoId = 1,
            CentroDistribucionId = 1,
            Stock = 50
        };

        // Act
        var resultado =
            await controller.CrearInventario(
                inventarioDuplicado);

        // Assert
        var conflictResult =
            Assert.IsType<ConflictObjectResult>(
                resultado.Result);

        Assert.Equal(
            "Ya existe inventario para este producto en este centro.",
            conflictResult.Value);
    }


    [Fact]
    public async Task CrearInventario_ConStockNegativoDevuelveBadRequest()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        var inventario = new InventarioProducto
        {
            ProductoId = 2,
            CentroDistribucionId = 1,
            Stock = -10
        };

        // Act
        var resultado =
            await controller.CrearInventario(
                inventario);

        // Assert
        var badRequestResult =
            Assert.IsType<BadRequestObjectResult>(
                resultado.Result);

        Assert.Equal(
            "El stock no puede ser negativo.",
            badRequestResult.Value);
    }


    [Fact]
    public async Task ActualizarInventario_CambiaStockYGeneraHistorial()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        var inventarioActualizado =
            new InventarioProducto
            {
                Id = 1,
                ProductoId = 1,
                CentroDistribucionId = 1,
                Stock = 45
            };

        // Act
        var resultado =
            await controller.ActualizarInventario(
                1,
                inventarioActualizado);

        // Assert
        Assert.IsType<NoContentResult>(resultado);

        var inventario =
            await context.InventariosProductos
                .SingleAsync(i => i.Id == 1);

        Assert.Equal(
            45,
            inventario.Stock);

        var historial =
            await context.InventarioHistorial
                .SingleAsync();

        Assert.Equal(
            20,
            historial.StockAnterior);

        Assert.Equal(
            45,
            historial.StockNuevo);

        Assert.Equal(
            25,
            historial.Diferencia);

        Assert.Equal(
            "ACTUALIZACION",
            historial.TipoMovimiento);
    }


    [Fact]
    public async Task ActualizarInventario_CuandoIdNoCoincideDevuelveBadRequest()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        var inventario =
            new InventarioProducto
            {
                Id = 2,
                ProductoId = 1,
                CentroDistribucionId = 1,
                Stock = 50
            };

        // Act
        var resultado =
            await controller.ActualizarInventario(
                1,
                inventario);

        // Assert
        var badRequestResult =
            Assert.IsType<BadRequestObjectResult>(
                resultado);

        Assert.Equal(
            "El ID de la URL no coincide con el ID del inventario.",
            badRequestResult.Value);
    }


    [Fact]
    public async Task ActualizarInventario_CuandoNoExisteDevuelveNotFound()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        var inventario =
            new InventarioProducto
            {
                Id = 999,
                ProductoId = 1,
                CentroDistribucionId = 1,
                Stock = 50
            };

        // Act
        var resultado =
            await controller.ActualizarInventario(
                999,
                inventario);

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                resultado);

        Assert.Equal(
            "El registro de inventario no existe.",
            notFoundResult.Value);
    }


    [Fact]
    public async Task EliminarInventario_CuandoExiste_EliminaYGeneraHistorial()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        // Act
        var resultado =
            await controller.EliminarInventario(1);

        // Assert
        Assert.IsType<NoContentResult>(resultado);

        var inventario =
            await context.InventariosProductos
                .FirstOrDefaultAsync(i => i.Id == 1);

        Assert.Null(inventario);

        var historial =
            await context.InventarioHistorial
                .SingleAsync();

        Assert.Equal(
            "ELIMINACION",
            historial.TipoMovimiento);

        Assert.Equal(
            20,
            historial.StockAnterior);

        Assert.Equal(
            0,
            historial.StockNuevo);

        Assert.Equal(
            -20,
            historial.Diferencia);
    }


    [Fact]
    public async Task EliminarInventario_CuandoNoExisteDevuelveNotFound()
    {
        // Arrange
        await using var context = CrearContexto();

        AgregarDatosBase(context);

        var controller = CrearController(context);

        // Act
        var resultado =
            await controller.EliminarInventario(999);

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                resultado);

        Assert.Equal(
            "El registro de inventario no existe.",
            notFoundResult.Value);
    }


    [Fact]
public async Task CrearInventario_CuandoProductoNoExisteDevuelveBadRequest()
{
    // Arrange
    await using var context = CrearContexto();

    AgregarDatosBase(context);

    var controller = CrearController(context);

    var inventario = new InventarioProducto
    {
        ProductoId = 999,
        CentroDistribucionId = 1,
        Stock = 10
    };

    // Act
    var resultado =
        await controller.CrearInventario(inventario);

    // Assert
    var badRequestResult =
        Assert.IsType<BadRequestObjectResult>(
            resultado.Result);

    Assert.Equal(
        "El producto indicado no existe.",
        badRequestResult.Value);
}



[Fact]
public async Task CrearInventario_CuandoCentroNoExisteDevuelveBadRequest()
{
    // Arrange
    await using var context = CrearContexto();

    AgregarDatosBase(context);

    var controller = CrearController(context);

    var inventario = new InventarioProducto
    {
        ProductoId = 2,
        CentroDistribucionId = 999,
        Stock = 10
    };

    // Act
    var resultado =
        await controller.CrearInventario(inventario);

    // Assert
    var badRequestResult =
        Assert.IsType<BadRequestObjectResult>(
            resultado.Result);

    Assert.Equal(
        "El centro de distribución indicado no existe.",
        badRequestResult.Value);
}




[Fact]
public async Task ActualizarInventario_ConStockNegativoDevuelveBadRequest()
{
    // Arrange
    await using var context = CrearContexto();

    AgregarDatosBase(context);

    var controller = CrearController(context);

    var inventario =
        new InventarioProducto
        {
            Id = 1,
            ProductoId = 1,
            CentroDistribucionId = 1,
            Stock = -10
        };

    // Act
    var resultado =
        await controller.ActualizarInventario(
            1,
            inventario);

    // Assert
    var badRequestResult =
        Assert.IsType<BadRequestObjectResult>(
            resultado);

    Assert.Equal(
        "El stock no puede ser negativo.",
        badRequestResult.Value);
}



[Fact]
public async Task ObtenerHistorial_CuandoHayDatosDevuelveOk()
{
    // Arrange
    await using var context = CrearContexto();

    AgregarDatosBase(context);

    context.InventarioHistorial.Add(
        new InventarioHistorial
        {
            ProductoId = 1,
            CentroDistribucionId = 1,
            StockAnterior = 20,
            StockNuevo = 30,
            Diferencia = 10,
            TipoMovimiento = "ACTUALIZACION",
            FechaMovimiento = DateTime.UtcNow
        });

    await context.SaveChangesAsync();

    var controller = CrearController(context);

    // Act
    var resultado =
        await controller.ObtenerHistorial();

    // Assert
    var okResult =
        Assert.IsType<OkObjectResult>(
            resultado.Result);

    var datos =
        Assert.IsAssignableFrom<IEnumerable<object>>(
            okResult.Value);

    Assert.Single(datos);
}



[Fact]
public async Task ObtenerHistorialPorProducto_CuandoHayDatosDevuelveOk()
{
    // Arrange
    await using var context = CrearContexto();

    AgregarDatosBase(context);

    context.InventarioHistorial.Add(
        new InventarioHistorial
        {
            ProductoId = 1,
            CentroDistribucionId = 1,
            StockAnterior = 20,
            StockNuevo = 30,
            Diferencia = 10,
            TipoMovimiento = "ACTUALIZACION",
            FechaMovimiento = DateTime.UtcNow
        });

    await context.SaveChangesAsync();

    var controller = CrearController(context);

    // Act
    var resultado =
        await controller.ObtenerHistorialPorProducto(1);

    // Assert
    var okResult =
        Assert.IsType<OkObjectResult>(
            resultado.Result);

    var datos =
        Assert.IsAssignableFrom<IEnumerable<object>>(
            okResult.Value);

    Assert.Single(datos);
}


[Fact]
public async Task ObtenerHistorialPorCentro_CuandoHayDatosDevuelveOk()
{
    // Arrange
    await using var context = CrearContexto();

    AgregarDatosBase(context);

    context.InventarioHistorial.Add(
        new InventarioHistorial
        {
            ProductoId = 1,
            CentroDistribucionId = 1,
            StockAnterior = 20,
            StockNuevo = 30,
            Diferencia = 10,
            TipoMovimiento = "ACTUALIZACION",
            FechaMovimiento = DateTime.UtcNow
        });

    await context.SaveChangesAsync();

    var controller = CrearController(context);

    // Act
    var resultado =
        await controller.ObtenerHistorialPorCentro(1);

    // Assert
    var okResult =
        Assert.IsType<OkObjectResult>(
            resultado.Result);

    var datos =
        Assert.IsAssignableFrom<IEnumerable<object>>(
            okResult.Value);

    Assert.Single(datos);
}

    private static TechLogisticsDbContext CrearContexto()
    {
        var options =
            new DbContextOptionsBuilder<TechLogisticsDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

        return new TechLogisticsDbContext(options);
    }


    private static InventarioController CrearController(
        TechLogisticsDbContext context)
    {
        return new InventarioController(
            context,
            NullLogger<InventarioController>.Instance);
    }


    private static void AgregarDatosBase(
        TechLogisticsDbContext context)
    {
        var producto1 = new Producto
        {
            Id = 1,
            Nombre = "Producto A",
            Categoria = "Pruebas",
            StockMinimo = 5,
            Activo = true
        };

        var producto2 = new Producto
        {
            Id = 2,
            Nombre = "Producto B",
            Categoria = "Pruebas",
            StockMinimo = 5,
            Activo = true
        };

        var centro = new CentroDistribucion
        {
            Id = 1,
            Nombre = "Centro Guatemala",
            Inventario = 20,
            EnLinea = true
        };

        var inventario = new InventarioProducto
        {
            Id = 1,
            ProductoId = 1,
            CentroDistribucionId = 1,
            Stock = 20,
            UltimaActualizacion = DateTime.UtcNow
        };

        context.Productos.AddRange(
            producto1,
            producto2);

        context.CentrosDistribucion.Add(centro);

        context.InventariosProductos.Add(
            inventario);

        context.SaveChanges();
    }
}