using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TechLogistics.Data;
using TechLogistics.Models;
using TechLogistics.Services;

namespace TechLogistics.Tests;

public class InventarioServiceTests
{
    [Fact]
    public async Task ActualizarInventarioAsync_GeneraHistorialYActualizaCentro()
    {
        // Arrange
        await using var context = CrearContexto();

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Producto de prueba",
            Categoria = "Pruebas",
            StockMinimo = 5,
            Activo = true
        };

        var centro = new CentroDistribucion
        {
            Id = 1,
            Nombre = "Centro de prueba",
            Inventario = 10,
            EnLinea = true
        };

        var inventario = new InventarioProducto
        {
            Id = 1,
            ProductoId = producto.Id,
            CentroDistribucionId = centro.Id,
            Stock = 10,
            UltimaActualizacion = DateTime.UtcNow.AddMinutes(-5)
        };

        context.Productos.Add(producto);
        context.CentrosDistribucion.Add(centro);
        context.InventariosProductos.Add(inventario);

        await context.SaveChangesAsync();

        var service = CrearService(context);

        // Act
        var resultado =
            await service.ActualizarInventarioAsync(0, 0);

        // Assert
        Assert.Single(resultado);

        var inventarioGuardado =
            await context.InventariosProductos.SingleAsync();

        Assert.Equal(10, inventarioGuardado.Stock);
        Assert.NotEqual(
            default,
            inventarioGuardado.UltimaActualizacion);

        var centroGuardado =
            await context.CentrosDistribucion.SingleAsync();

        Assert.Equal(10, centroGuardado.Inventario);

        var historial =
            await context.InventarioHistorial.SingleAsync();

        Assert.Equal(1, historial.ProductoId);
        Assert.Equal(1, historial.CentroDistribucionId);
        Assert.Equal(10, historial.StockAnterior);
        Assert.Equal(10, historial.StockNuevo);
        Assert.Equal(0, historial.Diferencia);
        Assert.Equal("SIMULACION", historial.TipoMovimiento);
    }


    [Fact]
    public async Task ActualizarInventarioAsync_ConVariosProductos_CalculaTotalCentro()
    {
        // Arrange
        await using var context = CrearContexto();

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
            Inventario = 0,
            EnLinea = true
        };

        var inventario1 = new InventarioProducto
        {
            Id = 1,
            ProductoId = 1,
            CentroDistribucionId = 1,
            Stock = 10
        };

        var inventario2 = new InventarioProducto
        {
            Id = 2,
            ProductoId = 2,
            CentroDistribucionId = 1,
            Stock = 25
        };

        context.Productos.AddRange(producto1, producto2);
        context.CentrosDistribucion.Add(centro);
        context.InventariosProductos.AddRange(
            inventario1,
            inventario2);

        await context.SaveChangesAsync();

        var service = CrearService(context);

        // Act
        var resultado =
            await service.ActualizarInventarioAsync(0, 0);

        // Assert
        Assert.Single(resultado);

        var centroGuardado =
            await context.CentrosDistribucion.SingleAsync();

        // 10 + 25 = 35
        Assert.Equal(35, centroGuardado.Inventario);

        var historiales =
            await context.InventarioHistorial
                .ToListAsync();

        Assert.Equal(2, historiales.Count);
    }


    [Fact]
    public async Task ActualizarInventarioAsync_NuncaPermiteStockNegativo()
    {
        // Arrange
        await using var context = CrearContexto();

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Producto con stock bajo",
            Categoria = "Pruebas",
            StockMinimo = 1,
            Activo = true
        };

        var centro = new CentroDistribucion
        {
            Id = 1,
            Nombre = "Centro de prueba",
            Inventario = 1,
            EnLinea = true
        };

        var inventario = new InventarioProducto
        {
            Id = 1,
            ProductoId = 1,
            CentroDistribucionId = 1,
            Stock = 1
        };

        context.Productos.Add(producto);
        context.CentrosDistribucion.Add(centro);
        context.InventariosProductos.Add(inventario);

        await context.SaveChangesAsync();

        var service = CrearService(context);

        // Act
        // -20 puede llevar el stock a valores negativos.
        // El servicio debe convertirlo en 0.
        await service.ActualizarInventarioAsync(-20, -20);

        // Assert
        var inventarioGuardado =
            await context.InventariosProductos.SingleAsync();

        Assert.Equal(0, inventarioGuardado.Stock);

        var historial =
            await context.InventarioHistorial.SingleAsync();

        Assert.Equal(1, historial.StockAnterior);
        Assert.Equal(0, historial.StockNuevo);
        Assert.Equal(-1, historial.Diferencia);
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


    private static InventarioService CrearService(
        TechLogisticsDbContext context)
    {
        return new InventarioService(
            context,
            NullLogger<InventarioService>.Instance);
    }
}