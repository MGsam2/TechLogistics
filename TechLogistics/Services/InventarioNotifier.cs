namespace TechLogistics.Services;

public class InventarioNotifier
{
    public event Func<InventarioActualizadoEvent, Task>? InventarioActualizado;

    public async Task NotificarActualizacion(
        InventarioActualizadoEvent evento)
    {
        if (InventarioActualizado is not null)
        {
            await InventarioActualizado.Invoke(evento);
        }
    }
}