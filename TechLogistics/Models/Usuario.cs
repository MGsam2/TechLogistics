using System.ComponentModel.DataAnnotations;

namespace TechLogistics.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    [Required]
    public int RolId { get; set; }

    public Rol? Rol { get; set; }
}
