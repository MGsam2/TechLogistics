namespace TechLogistics.Client.Models;

public class InventarioProducto
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public int CentroDistribucionId { get; set; }

    public int Stock { get; set; }

    public DateTime UltimaActualizacion { get; set; }

    public string? ProductoNombre { get; set; }

    public string? CentroNombre { get; set; }

    public Producto? Producto { get; set; }

    public CentroDistribucion? CentroDistribucion { get; set; }
}