using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechLogistics.Data;
using TechLogistics.Models;
using Microsoft.AspNetCore.Authorization;

namespace TechLogistics.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GerenteBodega")]
public class CatalogoController : ControllerBase
{
    private readonly TechLogisticsDbContext context;
    private readonly ILogger<CatalogoController> logger;

    public CatalogoController(
        TechLogisticsDbContext context,
        ILogger<CatalogoController> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    // GET: api/catalogo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> ObtenerProductos()
    {
        var productos = await context.Productos
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();

        return Ok(productos);
    }

    // GET: api/catalogo/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> ObtenerProducto(int id)
    {
        var producto = await context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto is null)
        {
            return NotFound();
        }

        return Ok(producto);
    }

    // POST: api/catalogo
    [HttpPost]
    public async Task<ActionResult<Producto>> CrearProducto(
        Producto producto)
    {
        context.Productos.Add(producto);

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Producto creado: {ProductoId} - {Nombre}",
            producto.Id,
            producto.Nombre);

        return CreatedAtAction(
            nameof(ObtenerProducto),
            new { id = producto.Id },
            producto);
    }

    // PUT: api/catalogo/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarProducto(
        int id,
        Producto producto)
    {
        if (id != producto.Id)
        {
            return BadRequest(
                "El ID de la URL no coincide con el ID del producto.");
        }

        var productoExistente =
            await context.Productos.FindAsync(id);

        if (productoExistente is null)
        {
            return NotFound();
        }

        productoExistente.Nombre = producto.Nombre;
        productoExistente.Categoria = producto.Categoria;
        productoExistente.StockMinimo = producto.StockMinimo;
        productoExistente.Activo = producto.Activo;

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Producto actualizado: {ProductoId}",
            producto.Id);

        return NoContent();
    }

    // DELETE: api/catalogo/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarProducto(int id)
    {
        var producto =
            await context.Productos.FindAsync(id);

        if (producto is null)
        {
            return NotFound();
        }

        context.Productos.Remove(producto);

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Producto eliminado: {ProductoId}",
            id);

        return NoContent();
    }
}