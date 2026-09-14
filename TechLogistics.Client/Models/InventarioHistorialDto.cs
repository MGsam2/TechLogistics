namespace TechLogistics.Client.Models;

public class InventarioHistorialDto
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public string? ProductoNombre { get; set; }

    public int CentroDistribucionId { get; set; }

    public string? CentroNombre { get; set; }

    public int StockAnterior { get; set; }

    public int StockNuevo { get; set; }

    public int Diferencia { get; set; }

    public string TipoMovimiento { get; set; } = string.Empty;

    public DateTime FechaMovimiento { get; set; }
}