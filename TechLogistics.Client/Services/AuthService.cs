using System.Net.Http.Json;
using TechLogistics.Client.Models;

namespace TechLogistics.Client.Services;

public class AuthService
{
    private readonly HttpClient httpClient;

    public AuthService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<LoginResponse?> LoginAsync(
        string nombreUsuario,
        string password)
    {
        var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest
            {
                NombreUsuario = nombreUsuario,
                Password = password
            });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<LoginResponse>();
    }
}

public class LoginRequest
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
}