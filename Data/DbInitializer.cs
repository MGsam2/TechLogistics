using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechLogistics.Models;

namespace TechLogistics.Data;

public static class DbInitializer
{
    public static async Task InicializarAsync(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<TechLogisticsDbContext>();

        var passwordHasher =
            scope.ServiceProvider
                .GetRequiredService<IPasswordHasher<Usuario>>();

        await context.Database.MigrateAsync();

        var rolGerente = await context.Roles
            .FirstOrDefaultAsync(r =>
                r.Nombre == "GerenteBodega");

        var rolAgente = await context.Roles
            .FirstOrDefaultAsync(r =>
                r.Nombre == "AgenteCampo");

        if (rolGerente is null || rolAgente is null)
        {
            return;
        }

        // ==========================================
        // USUARIO GERENTE
        // ==========================================

        var gerenteExistente =
            await context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.NombreUsuario == "gerente");

        if (gerenteExistente is null)
        {
            var gerente = new Usuario
            {
                NombreUsuario = "gerente",
                NombreCompleto = "Gerente de Bodega",
                Activo = true,
                RolId = rolGerente.Id
            };

            gerente.PasswordHash =
                passwordHasher.HashPassword(
                    gerente,
                    "Gerente123");

            context.Usuarios.Add(gerente);
        }

        // ==========================================
        // USUARIO AGENTE
        // ==========================================

        var agenteExistente =
            await context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.NombreUsuario == "agente");

        if (agenteExistente is null)
        {
            var agente = new Usuario
            {
                NombreUsuario = "agente",
                NombreCompleto = "Agente de Campo",
                Activo = true,
                RolId = rolAgente.Id
            };

            agente.PasswordHash =
                passwordHasher.HashPassword(
                    agente,
                    "Agente123");

            context.Usuarios.Add(agente);
        }

        await context.SaveChangesAsync();
    }
}