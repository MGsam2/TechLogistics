using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TechLogistics.Services;

public class PreferenciasUsuarioService
{
    private readonly ProtectedLocalStorage localStorage;

    private const string UltimoCentroKey =
        "techlogistics.ultimoCentro";

    public PreferenciasUsuarioService(
        ProtectedLocalStorage localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task GuardarUltimoCentroAsync(
        string nombreCentro)
    {
        await localStorage.SetAsync(
            UltimoCentroKey,
            nombreCentro);
    }

    public async Task<string?> ObtenerUltimoCentroAsync()
    {
        var resultado =
            await localStorage.GetAsync<string>(
                UltimoCentroKey);

        return resultado.Success
            ? resultado.Value
            : null;
    }

    public async Task EliminarUltimoCentroAsync()
    {
        await localStorage.DeleteAsync(
            UltimoCentroKey);
    }
}