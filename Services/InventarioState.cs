using System.ComponentModel;
using TechLogistics.Models;

namespace TechLogistics.Services;

public class InventarioState : INotifyPropertyChanged
{
    private List<CentroDistribucion> centros = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<CentroDistribucion> Centros =>
        centros;

    public int TotalCentros =>
        centros.Count;

    public int CentrosEnLinea =>
        centros.Count(c => c.EnLinea);

    public int CentrosEnAdvertencia =>
        centros.Count(c => !c.EnLinea);

    public int InventarioTotal =>
        centros.Sum(c => c.Inventario);

    public void ActualizarCentros(
        IEnumerable<CentroDistribucion> nuevosCentros)
    {
        centros = nuevosCentros.ToList();

        NotificarCambio(nameof(Centros));
        NotificarCambio(nameof(TotalCentros));
        NotificarCambio(nameof(CentrosEnLinea));
        NotificarCambio(nameof(CentrosEnAdvertencia));
        NotificarCambio(nameof(InventarioTotal));
    }

    private void NotificarCambio(string propiedad)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propiedad));
    }
}