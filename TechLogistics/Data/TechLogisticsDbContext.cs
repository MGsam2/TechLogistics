using Microsoft.EntityFrameworkCore;
using TechLogistics.Models;

namespace TechLogistics.Data;

public class TechLogisticsDbContext : DbContext
{
    public TechLogisticsDbContext(
        DbContextOptions<TechLogisticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventarioHistorial> InventarioHistorial
    => Set<InventarioHistorial>();

    public DbSet<CentroDistribucion> CentrosDistribucion
        => Set<CentroDistribucion>();

    public DbSet<Producto> Productos
        => Set<Producto>();

    public DbSet<InventarioProducto> InventariosProductos
        => Set<InventarioProducto>();

    public DbSet<Usuario> Usuarios
        => Set<Usuario>();

    public DbSet<Rol> Roles
        => Set<Rol>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================
        // INVENTARIO -> PRODUCTO
        // =====================================================

        modelBuilder.Entity<InventarioProducto>()
            .HasOne(i => i.Producto)
            .WithMany(p => p.Inventarios)
            .HasForeignKey(i => i.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

        // =====================================================
        // INVENTARIO -> CENTRO
        // =====================================================

        modelBuilder.Entity<InventarioProducto>()
            .HasOne(i => i.CentroDistribucion)
            .WithMany(c => c.Inventarios)
            .HasForeignKey(i => i.CentroDistribucionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioProducto>()
            .HasIndex(i => new
            {
                i.ProductoId,
                i.CentroDistribucionId
            })
            .IsUnique();

            // =====================================================
            // HISTORIAL -> PRODUCTO
            // =====================================================

            modelBuilder.Entity<InventarioHistorial>()
                .HasOne(h => h.Producto)
                .WithMany()
                .HasForeignKey(h => h.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // HISTORIAL -> CENTRO
            // =====================================================

            modelBuilder.Entity<InventarioHistorial>()
                .HasOne(h => h.CentroDistribucion)
                .WithMany()
                .HasForeignKey(h => h.CentroDistribucionId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // ÍNDICES DEL HISTORIAL
            // =====================================================

            modelBuilder.Entity<InventarioHistorial>()
                .HasIndex(h => h.FechaMovimiento);

            modelBuilder.Entity<InventarioHistorial>()
                .HasIndex(h => new
                {
                    h.ProductoId,
                    h.CentroDistribucionId
                });

        // =====================================================
        // ROL -> USUARIOS
        // =====================================================

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // CONFIGURACIÓN DE USUARIO
        // =====================================================

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.NombreUsuario)
            .IsUnique();

        // =====================================================
        // ROLES INICIALES
        // =====================================================

        modelBuilder.Entity<Rol>().HasData(
            new Rol
            {
                Id = 1,
                Nombre = "GerenteBodega"
            },
            new Rol
            {
                Id = 2,
                Nombre = "AgenteCampo"
            });
    }
}