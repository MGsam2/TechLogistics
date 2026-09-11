using System.ComponentModel;
using TechLogistics.Grpc;

namespace TechLogistics.Client.Services;

public class InventarioClientState : INotifyPropertyChanged
{
    private readonly Dictionary<int, InventarioActualizacion> centros = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyCollection<InventarioActualizacion> Centros =>
        centros.Values;

    public int TotalCentros =>
        centros.Count;

    public int CentrosEnLinea =>
        centros.Values.Count(c => c.EnLinea);

    public int CentrosEnAdvertencia =>
        centros.Values.Count(c => !c.EnLinea);

    public int InventarioTotal =>
        centros.Values.Sum(c => c.InventarioTotal);

    public void Actualizar(
        InventarioActualizacion actualizacion)
    {
        centros[actualizacion.CentroId] = actualizacion;

        Notificar(nameof(Centros));
        Notificar(nameof(TotalCentros));
        Notificar(nameof(CentrosEnLinea));
        Notificar(nameof(CentrosEnAdvertencia));
        Notificar(nameof(InventarioTotal));
    }

    public void Limpiar()
    {
        centros.Clear();

        Notificar(nameof(Centros));
        Notificar(nameof(TotalCentros));
        Notificar(nameof(CentrosEnLinea));
        Notificar(nameof(CentrosEnAdvertencia));
        Notificar(nameof(InventarioTotal));
    }

    private void Notificar(string propiedad)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propiedad));
    }
}