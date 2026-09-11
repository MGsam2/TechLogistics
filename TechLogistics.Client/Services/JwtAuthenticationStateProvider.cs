using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace TechLogistics.Client.Services;

public class JwtAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly AuthService authService;
    private readonly IJSRuntime jsRuntime;

    private ClaimsPrincipal usuarioActual =
        new ClaimsPrincipal(
            new ClaimsIdentity());

    private bool estadoInicializado;

    public JwtAuthenticationStateProvider(
        AuthService authService,
        IJSRuntime jsRuntime)
    {
        this.authService = authService;
        this.jsRuntime = jsRuntime;
    }

    public override Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            new AuthenticationState(
                usuarioActual));
    }

    public async Task<bool> LoginAsync(
        string nombreUsuario,
        string password)
    {
        var resultado =
            await authService.LoginAsync(
                nombreUsuario,
                password);

        if (resultado is null)
        {
            return false;
        }

        await jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            "techlogistics_token",
            resultado.Token);

        usuarioActual =
            CrearUsuarioDesdeToken(
                resultado.Token);

        estadoInicializado = true;

        NotificarEstado();

        return true;
    }

    public async Task LogoutAsync()
    {
        await jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            "techlogistics_token");

        usuarioActual =
            UsuarioAnonimo();

        estadoInicializado = true;

        NotificarEstado();
    }

    public async Task InicializarAsync()
    {
        if (estadoInicializado)
        {
            return;
        }

        await CargarUsuarioAsync();

        estadoInicializado = true;

        NotificarEstado();
    }

    private async Task CargarUsuarioAsync()
    {
        try
        {
            var token =
                await jsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    "techlogistics_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                usuarioActual =
                    UsuarioAnonimo();

                return;
            }

            var handler =
                new JwtSecurityTokenHandler();

            var jwt =
                handler.ReadJwtToken(token);

            if (jwt.ValidTo <= DateTime.UtcNow)
            {
                await jsRuntime.InvokeVoidAsync(
                    "localStorage.removeItem",
                    "techlogistics_token");

                usuarioActual =
                    UsuarioAnonimo();

                return;
            }

            usuarioActual =
                CrearUsuarioDesdeToken(token);
        }
        catch
        {
            usuarioActual =
                UsuarioAnonimo();
        }
    }

    private static ClaimsPrincipal
        CrearUsuarioDesdeToken(string token)
    {
        var handler =
            new JwtSecurityTokenHandler();

        var jwt =
            handler.ReadJwtToken(token);

        var claims =
            jwt.Claims.ToList();

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                claims,
                authenticationType: "jwt",
                nameType: "unique_name",
                roleType: "role"));
    }

    private static ClaimsPrincipal
        UsuarioAnonimo()
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity());
    }

    private void NotificarEstado()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(
                    usuarioActual)));
    }
}
