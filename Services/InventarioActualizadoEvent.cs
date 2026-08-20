using TechLogistics.Models;

namespace TechLogistics.Services;

public class InventarioActualizadoEvent
{
    public CentroDistribucion Centro { get; }

    public InventarioActualizadoEvent(CentroDistribucion centro)
    {
        Centro = centro;
    }
}