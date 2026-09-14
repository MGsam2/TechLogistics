namespace TechLogistics.Client.Models;

public class OperacionOffline
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int InventarioId { get; set; }

    public int ProductoId { get; set; }

    public int CentroDistribucionId { get; set; }

    public int Stock { get; set; }

    public DateTime FechaCreacion { get; set; } =
        DateTime.UtcNow;

    public string TipoOperacion { get; set; } =
        "ACTUALIZACION";
}