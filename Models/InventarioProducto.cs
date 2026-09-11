using System.ComponentModel.DataAnnotations;

namespace TechLogistics.Models;

public class InventarioProducto
{
    public int Id { get; set; }

    [Required]
    public int ProductoId { get; set; }

    [Required]
    public int CentroDistribucionId { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public DateTime UltimaActualizacion { get; set; }

    public Producto? Producto { get; set; }

    public CentroDistribucion? CentroDistribucion { get; set; }
}