using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TechLogistics.Controllers;
using TechLogistics.Data;
using TechLogistics.Models;
using Xunit;

namespace TechLogistics.Tests.Controllers;

public class CatalogoControllerTests
{
    private static TechLogisticsDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<TechLogisticsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TechLogisticsDbContext(options);
    }

    [Fact]
    public async Task ObtenerProductos_DevuelveProductosOrdenadosPorId()
    {
        await using var context = CrearContexto();

        context.Productos.AddRange(
            new Producto
            {
                Id = 2,
                Nombre = "Producto B",
                Categoria = "Categoria",
                StockMinimo = 10,
                Activo = true
            },
            new Producto
            {
                Id = 1,
                Nombre = "Producto A",
                Categoria = "Categoria",
                StockMinimo = 5,
                Activo = true
            });

        await context.SaveChangesAsync();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var resultado = await controller.ObtenerProductos();

        var ok = Assert.IsType<
            Microsoft.AspNetCore.Mvc.OkObjectResult>(resultado.Result);

        var productos = Assert.IsAssignableFrom<
            IEnumerable<Producto>>(ok.Value);

        Assert.Equal(2, productos.Count());
        Assert.Equal(1, productos.First().Id);
        Assert.Equal(2, productos.Last().Id);
    }

    [Fact]
    public async Task ObtenerProducto_Existente_DevuelveOk()
    {
        await using var context = CrearContexto();

        context.Productos.Add(
            new Producto
            {
                Id = 1,
                Nombre = "Producto A",
                Categoria = "Electronica",
                StockMinimo = 10,
                Activo = true
            });

        await context.SaveChangesAsync();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var resultado = await controller.ObtenerProducto(1);

        var ok = Assert.IsType<
            Microsoft.AspNetCore.Mvc.OkObjectResult>(resultado.Result);

        var producto = Assert.IsType<Producto>(ok.Value);

        Assert.Equal(1, producto.Id);
        Assert.Equal("Producto A", producto.Nombre);
    }

    [Fact]
    public async Task ObtenerProducto_Inexistente_DevuelveNotFound()
    {
        await using var context = CrearContexto();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var resultado = await controller.ObtenerProducto(999);

        Assert.IsType<
            Microsoft.AspNetCore.Mvc.NotFoundResult>(resultado.Result);
    }

    [Fact]
    public async Task CrearProducto_CreaProductoYDevuelveCreated()
    {
        await using var context = CrearContexto();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var producto = new Producto
        {
            Nombre = "Producto Nuevo",
            Categoria = "Electronica",
            StockMinimo = 15,
            Activo = true
        };

        var resultado =
            await controller.CrearProducto(producto);

        var created = Assert.IsType<
            Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(
                resultado.Result);

        var productoCreado =
            Assert.IsType<Producto>(created.Value);

        Assert.Equal("Producto Nuevo", productoCreado.Nombre);
        Assert.Equal(15, productoCreado.StockMinimo);

        var productoEnDb =
            await context.Productos.SingleAsync();

        Assert.Equal("Producto Nuevo", productoEnDb.Nombre);
    }

    [Fact]
    public async Task ActualizarProducto_IdDiferente_DevuelveBadRequest()
    {
        await using var context = CrearContexto();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var producto = new Producto
        {
            Id = 2,
            Nombre = "Producto",
            Categoria = "Categoria",
            StockMinimo = 10,
            Activo = true
        };

        var resultado =
            await controller.ActualizarProducto(1, producto);

        var badRequest = Assert.IsType<
            Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(
                resultado);

        Assert.Contains(
            "ID de la URL",
            badRequest.Value?.ToString());
    }

    [Fact]
    public async Task ActualizarProducto_Inexistente_DevuelveNotFound()
    {
        await using var context = CrearContexto();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var producto = new Producto
        {
            Id = 999,
            Nombre = "Producto",
            Categoria = "Categoria",
            StockMinimo = 10,
            Activo = true
        };

        var resultado =
            await controller.ActualizarProducto(999, producto);

        Assert.IsType<
            Microsoft.AspNetCore.Mvc.NotFoundResult>(resultado);
    }

    [Fact]
    public async Task ActualizarProducto_Existente_ActualizaDatosYDevuelveNoContent()
    {
        await using var context = CrearContexto();

        context.Productos.Add(
            new Producto
            {
                Id = 1,
                Nombre = "Producto Original",
                Categoria = "Original",
                StockMinimo = 5,
                Activo = true
            });

        await context.SaveChangesAsync();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var productoActualizado = new Producto
        {
            Id = 1,
            Nombre = "Producto Actualizado",
            Categoria = "Nueva Categoria",
            StockMinimo = 20,
            Activo = false
        };

        var resultado =
            await controller.ActualizarProducto(
                1,
                productoActualizado);

        Assert.IsType<
            Microsoft.AspNetCore.Mvc.NoContentResult>(
                resultado);

        var productoEnDb =
            await context.Productos.FindAsync(1);

        Assert.NotNull(productoEnDb);
        Assert.Equal(
            "Producto Actualizado",
            productoEnDb.Nombre);
        Assert.Equal(
            "Nueva Categoria",
            productoEnDb.Categoria);
        Assert.Equal(20, productoEnDb.StockMinimo);
        Assert.False(productoEnDb.Activo);
    }

    [Fact]
    public async Task EliminarProducto_Inexistente_DevuelveNotFound()
    {
        await using var context = CrearContexto();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var resultado =
            await controller.EliminarProducto(999);

        Assert.IsType<
            Microsoft.AspNetCore.Mvc.NotFoundResult>(
                resultado);
    }

    [Fact]
    public async Task EliminarProducto_Existente_EliminaYDevuelveNoContent()
    {
        await using var context = CrearContexto();

        context.Productos.Add(
            new Producto
            {
                Id = 1,
                Nombre = "Producto A",
                Categoria = "Categoria",
                StockMinimo = 10,
                Activo = true
            });

        await context.SaveChangesAsync();

        var controller = new CatalogoController(
            context,
            NullLogger<CatalogoController>.Instance);

        var resultado =
            await controller.EliminarProducto(1);

        Assert.IsType<
            Microsoft.AspNetCore.Mvc.NoContentResult>(
                resultado);

        var producto =
            await context.Productos.FindAsync(1);

        Assert.Null(producto);
    }
}