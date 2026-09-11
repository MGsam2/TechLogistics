namespace TechLogistics.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public int UsuarioId { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public DateTime Expira { get; set; }
}