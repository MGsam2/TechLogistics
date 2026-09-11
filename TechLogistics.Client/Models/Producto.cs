namespace TechLogistics.Client.Models;

public class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public int StockMinimo { get; set; }

    public bool Activo { get; set; }
}