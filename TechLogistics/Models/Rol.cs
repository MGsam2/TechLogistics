using System.ComponentModel.DataAnnotations;

namespace TechLogistics.Models;

public class Rol
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; }
        = new List<Usuario>();
}