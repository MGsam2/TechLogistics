using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechLogistics.Data;
using TechLogistics.Models;
using Microsoft.AspNetCore.Authorization;

namespace TechLogistics.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GerenteBodega")]
public class CentroDistribucionController : ControllerBase
{
    private readonly TechLogisticsDbContext context;
    private readonly ILogger<CentroDistribucionController> logger;

    public CentroDistribucionController(
        TechLogisticsDbContext context,
        ILogger<CentroDistribucionController> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    // GET: api/centrodistribucion
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CentroDistribucion>>> ObtenerCentros()
    {
        var centros = await context.CentrosDistribucion
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();

        return Ok(centros);
    }

    // GET: api/centrodistribucion/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CentroDistribucion>> ObtenerCentro(int id)
    {
        var centro = await context.CentrosDistribucion
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (centro is null)
        {
            return NotFound(
                "El centro de distribución no existe.");
        }

        return Ok(centro);
    }

    // POST: api/centrodistribucion
    [HttpPost]
    public async Task<ActionResult<CentroDistribucion>> CrearCentro(
        CentroDistribucion centro)
    {
        if (string.IsNullOrWhiteSpace(centro.Nombre))
        {
            return BadRequest(
                "El nombre del centro es obligatorio.");
        }

        var nombreExiste = await context.CentrosDistribucion
            .AnyAsync(c => c.Nombre.ToLower() == centro.Nombre.ToLower());

        if (nombreExiste)
        {
            return Conflict(
                "Ya existe un centro de distribución con ese nombre.");
        }

        centro.UltimaActualizacion = DateTime.UtcNow;

        context.CentrosDistribucion.Add(centro);

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Centro creado: {CentroId} - {Nombre}",
            centro.Id,
            centro.Nombre);

        return CreatedAtAction(
            nameof(ObtenerCentro),
            new { id = centro.Id },
            centro);
    }

    // PUT: api/centrodistribucion/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarCentro(
        int id,
        CentroDistribucion centro)
    {
        if (id != centro.Id)
        {
            return BadRequest(
                "El ID de la URL no coincide con el ID del centro.");
        }

        if (string.IsNullOrWhiteSpace(centro.Nombre))
        {
            return BadRequest(
                "El nombre del centro es obligatorio.");
        }

        var centroExistente =
            await context.CentrosDistribucion.FindAsync(id);

        if (centroExistente is null)
        {
            return NotFound(
                "El centro de distribución no existe.");
        }

        var nombreExiste = await context.CentrosDistribucion
            .AnyAsync(c =>
                c.Id != id &&
                c.Nombre.ToLower() == centro.Nombre.ToLower());

        if (nombreExiste)
        {
            return Conflict(
                "Ya existe otro centro con ese nombre.");
        }

        centroExistente.Nombre = centro.Nombre;
        centroExistente.Inventario = centro.Inventario;
        centroExistente.EnLinea = centro.EnLinea;
        centroExistente.UltimaActualizacion = DateTime.UtcNow;

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Centro actualizado: {CentroId}",
            id);

        return NoContent();
    }

    // DELETE: api/centrodistribucion/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarCentro(int id)
    {
        var centro =
            await context.CentrosDistribucion.FindAsync(id);

        if (centro is null)
        {
            return NotFound(
                "El centro de distribución no existe.");
        }

        context.CentrosDistribucion.Remove(centro);

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Centro eliminado: {CentroId}",
            id);

        return NoContent();
    }

    // GET: api/centrodistribucion/1/inventario
    [HttpGet("{id:int}/inventario")]
    public async Task<ActionResult<IEnumerable<object>>> ObtenerInventarioCentro(
        int id)
    {
        var centroExiste = await context.CentrosDistribucion
            .AnyAsync(c => c.Id == id);

        if (!centroExiste)
        {
            return NotFound(
                "El centro de distribución no existe.");
        }

        var inventario = await context.InventariosProductos
            .AsNoTracking()
            .Where(i => i.CentroDistribucionId == id)
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
}