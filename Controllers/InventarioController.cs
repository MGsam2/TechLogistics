using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechLogistics.Data;
using TechLogistics.Models;

namespace TechLogistics.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GerenteBodega,AgenteCampo")]
public class InventarioController : ControllerBase
{
    private readonly TechLogisticsDbContext context;
    private readonly ILogger<InventarioController> logger;

    public InventarioController(
        TechLogisticsDbContext context,
        ILogger<InventarioController> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    // ============================================================
    // GET: api/inventario
    // Obtener todo el inventario
    // Acceso: GerenteBodega y AgenteCampo
    // ============================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> ObtenerInventario()
    {
        var inventario = await context.InventariosProductos
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Select(i => new
            {
                i.Id,
                i.ProductoId,

                ProductoNombre = i.Producto != null
                    ? i.Producto.Nombre
                    : null,

                i.CentroDistribucionId,

                CentroNombre = i.CentroDistribucion != null
                    ? i.CentroDistribucion.Nombre
                    : null,

                i.Stock,
                i.UltimaActualizacion
            })
            .ToListAsync();

        return Ok(inventario);
    }

    // ============================================================
    // GET: api/inventario/1
    // Obtener un registro específico
    // Acceso: GerenteBodega y AgenteCampo
    // ============================================================
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> ObtenerInventarioPorId(
        int id)
    {
        var inventario = await context.InventariosProductos
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new
            {
                i.Id,
                i.ProductoId,

                ProductoNombre = i.Producto != null
                    ? i.Producto.Nombre
                    : null,

                i.CentroDistribucionId,

                CentroNombre = i.CentroDistribucion != null
                    ? i.CentroDistribucion.Nombre
                    : null,

                i.Stock,
                i.UltimaActualizacion
            })
            .FirstOrDefaultAsync();

        if (inventario is null)
        {
            return NotFound(
                "El registro de inventario no existe.");
        }

        return Ok(inventario);
    }

    // ============================================================
    // GET: api/inventario/producto/1
    // Obtener inventario de un producto
    // Acceso: GerenteBodega y AgenteCampo
    // ============================================================
    [HttpGet("producto/{productoId:int}")]
    public async Task<ActionResult<IEnumerable<object>>> ObtenerPorProducto(
        int productoId)
    {
        var productoExiste = await context.Productos
            .AnyAsync(p => p.Id == productoId);

        if (!productoExiste)
        {
            return NotFound(
                "El producto indicado no existe.");
        }

        var inventario = await context.InventariosProductos
            .AsNoTracking()
            .Where(i => i.ProductoId == productoId)
            .OrderBy(i => i.CentroDistribucionId)
            .Select(i => new
            {
                i.Id,
                i.ProductoId,

                ProductoNombre = i.Producto != null
                    ? i.Producto.Nombre
                    : null,

                i.CentroDistribucionId,

                CentroNombre = i.CentroDistribucion != null
                    ? i.CentroDistribucion.Nombre
                    : null,

                i.Stock,
                i.UltimaActualizacion
            })
            .ToListAsync();

        return Ok(inventario);
    }

    // ============================================================
    // GET: api/inventario/centro/1
    // Obtener inventario de un centro
    // Acceso: GerenteBodega y AgenteCampo
    // ============================================================
    [HttpGet("centro/{centroId:int}")]
    public async Task<ActionResult<IEnumerable<object>>> ObtenerPorCentro(
        int centroId)
    {
        var centroExiste = await context.CentrosDistribucion
            .AnyAsync(c => c.Id == centroId);

        if (!centroExiste)
        {
            return NotFound(
                "El centro de distribución indicado no existe.");
        }

        var inventario = await context.InventariosProductos
            .AsNoTracking()
            .Where(i => i.CentroDistribucionId == centroId)
            .OrderBy(i => i.ProductoId)
            .Select(i => new
            {
                i.Id,
                i.ProductoId,

                ProductoNombre = i.Producto != null
                    ? i.Producto.Nombre
                    : null,

                i.CentroDistribucionId,

                CentroNombre = i.CentroDistribucion != null
                    ? i.CentroDistribucion.Nombre
                    : null,

                i.Stock,
                i.UltimaActualizacion
            })
            .ToListAsync();

        return Ok(inventario);
    }

    // ============================================================
    // POST: api/inventario
    // Crear inventario
    // Acceso: SOLO GerenteBodega
    // ============================================================
    [Authorize(Roles = "GerenteBodega")]
    [HttpPost]
    public async Task<ActionResult<object>> CrearInventario(
        InventarioProducto inventario)
    {
        var producto = await context.Productos
            .FirstOrDefaultAsync(
                p => p.Id == inventario.ProductoId);

        if (producto is null)
        {
            return BadRequest(
                "El producto indicado no existe.");
        }

        var centro = await context.CentrosDistribucion
            .FirstOrDefaultAsync(
                c => c.Id == inventario.CentroDistribucionId);

        if (centro is null)
        {
            return BadRequest(
                "El centro de distribución indicado no existe.");
        }

        if (inventario.Stock < 0)
        {
            return BadRequest(
                "El stock no puede ser negativo.");
        }

        var existe = await context.InventariosProductos
            .AnyAsync(i =>
                i.ProductoId == inventario.ProductoId &&
                i.CentroDistribucionId ==
                    inventario.CentroDistribucionId);

        if (existe)
        {
            return Conflict(
                "Ya existe inventario para este producto en este centro.");
        }

        inventario.UltimaActualizacion =
            DateTime.UtcNow;

        context.InventariosProductos.Add(inventario);

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Inventario creado: Producto {ProductoId}, Centro {CentroId}, Stock {Stock}",
            inventario.ProductoId,
            inventario.CentroDistribucionId,
            inventario.Stock);

        var resultado = new
        {
            inventario.Id,
            inventario.ProductoId,
            ProductoNombre = producto.Nombre,
            inventario.CentroDistribucionId,
            CentroNombre = centro.Nombre,
            inventario.Stock,
            inventario.UltimaActualizacion
        };

        return CreatedAtAction(
            nameof(ObtenerInventarioPorId),
            new { id = inventario.Id },
            resultado);
    }

    // ============================================================
    // PUT: api/inventario/1
    // Actualizar stock
    // Acceso: SOLO GerenteBodega
    // ============================================================
    [Authorize(Roles = "GerenteBodega")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarInventario(
        int id,
        InventarioProducto inventario)
    {
        if (id != inventario.Id)
        {
            return BadRequest(
                "El ID de la URL no coincide con el ID del inventario.");
        }

        if (inventario.Stock < 0)
        {
            return BadRequest(
                "El stock no puede ser negativo.");
        }

        var inventarioExistente =
            await context.InventariosProductos
                .FindAsync(id);

        if (inventarioExistente is null)
        {
            return NotFound(
                "El registro de inventario no existe.");
        }

        inventarioExistente.Stock =
            inventario.Stock;

        inventarioExistente.UltimaActualizacion =
            DateTime.UtcNow;

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Inventario actualizado: {InventarioId}, Stock {Stock}",
            id,
            inventario.Stock);

        return NoContent();
    }

    // ============================================================
    // DELETE: api/inventario/1
    // Eliminar inventario
    // Acceso: SOLO GerenteBodega
    // ============================================================
    [Authorize(Roles = "GerenteBodega")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarInventario(
        int id)
    {
        var inventario =
            await context.InventariosProductos
                .FindAsync(id);

        if (inventario is null)
        {
            return NotFound(
                "El registro de inventario no existe.");
        }

        context.InventariosProductos.Remove(inventario);

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Inventario eliminado: {InventarioId}",
            id);

        return NoContent();
    }
}