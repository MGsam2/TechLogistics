namespace TechLogistics.Models;

public class CentroDistribucion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int Inventario { get; set; }

    public bool EnLinea { get; set; }
    public DateTime UltimaActualizacion { get; set; }
}