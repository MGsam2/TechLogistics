using System.ComponentModel.DataAnnotations;

namespace TechLogistics.Models;

public class InventarioHistorial
{
    public int Id { get; set; }

    [Required]
    public int ProductoId { get; set; }

    [Required]
    public int CentroDistribucionId { get; set; }

    public int StockAnterior { get; set; }

    public int StockNuevo { get; set; }

    public int Diferencia { get; set; }

    [Required]
    [MaxLength(30)]
    public string TipoMovimiento { get; set; } = string.Empty;

    public DateTime FechaMovimiento { get; set; }

    public Producto? Producto { get; set; }

    public CentroDistribucion? CentroDistribucion { get; set; }
}