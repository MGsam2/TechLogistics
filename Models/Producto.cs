using System.ComponentModel.DataAnnotations;

namespace TechLogistics.Models;

public class Producto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    public int StockMinimo { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<InventarioProducto> Inventarios { get; set; }
        = new List<InventarioProducto>();
}