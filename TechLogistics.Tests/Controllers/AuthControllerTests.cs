using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using TechLogistics.Controllers;
using TechLogistics.Data;
using TechLogistics.DTOs;
using TechLogistics.Models;
using TechLogistics.Services;
using Xunit;

namespace TechLogistics.Tests.Controllers;

public class AuthControllerTests
{
    private static TechLogisticsDbContext CrearContexto()
    {
        var options =
            new DbContextOptionsBuilder<TechLogisticsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new TechLogisticsDbContext(options);
    }

    private static JwtService CrearJwtService()
    {
        var configuracion =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] =
                            "clave-secreta-para-pruebas-techlogistics-123456",

                        ["Jwt:Issuer"] =
                            "TechLogistics",

                        ["Jwt:Audience"] =
                            "TechLogisticsClient",

                        ["Jwt:ExpirationMinutes"] =
                            "120"
                    })
                .Build();

        return new JwtService(configuracion);
    }

    private static AuthController CrearController(
        TechLogisticsDbContext context,
        IPasswordHasher<Usuario> passwordHasher)
    {
        return new AuthController(
            context,
            CrearJwtService(),
            passwordHasher,
            NullLogger<AuthController>.Instance);
    }

    private static Usuario CrearUsuarioGerente(
        int id = 1)
    {
        return new Usuario
        {
            Id = id,

            NombreUsuario =
                "gerente",

            NombreCompleto =
                "Gerente de Bodega",

            Activo = true,

            RolId = 1,

            Rol = new Rol
            {
                Id = 1,
                Nombre = "GerenteBodega"
            }
        };
    }

    [Fact]
    public async Task Login_UsuarioInexistente_DevuelveUnauthorized()
    {
        await using var context = CrearContexto();

        var passwordHasher =
            new PasswordHasher<Usuario>();

        var controller =
            CrearController(
                context,
                passwordHasher);

        var request = new LoginRequest
        {
            NombreUsuario =
                "usuario.inexistente",

            Password =
                "Password123!"
        };

        var resultado =
            await controller.Login(request);

        var unauthorized =
            Assert.IsType<UnauthorizedObjectResult>(
                resultado.Result);

        Assert.Equal(
            "Usuario o contraseña incorrectos.",
            unauthorized.Value);
    }

    [Fact]
    public async Task Login_UsuarioInactivo_DevuelveUnauthorized()
    {
        await using var context = CrearContexto();

        var passwordHasher =
            new PasswordHasher<Usuario>();

        var usuario =
            CrearUsuarioGerente();

        usuario.NombreUsuario =
            "gerente.inactivo";

        usuario.NombreCompleto =
            "Gerente Inactivo";

        usuario.Activo = false;

        usuario.PasswordHash =
            passwordHasher.HashPassword(
                usuario,
                "Password123!");

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        var controller =
            CrearController(
                context,
                passwordHasher);

        var request = new LoginRequest
        {
            NombreUsuario =
                "gerente.inactivo",

            Password =
                "Password123!"
        };

        var resultado =
            await controller.Login(request);

        var unauthorized =
            Assert.IsType<UnauthorizedObjectResult>(
                resultado.Result);

        Assert.Equal(
            "Usuario o contraseña incorrectos.",
            unauthorized.Value);
    }

    [Fact]
    public async Task Login_ContraseñaIncorrecta_DevuelveUnauthorized()
    {
        await using var context = CrearContexto();

        var passwordHasher =
            new PasswordHasher<Usuario>();

        var usuario =
            CrearUsuarioGerente();

        usuario.NombreUsuario =
            "gerente.prueba";

        usuario.NombreCompleto =
            "Gerente Prueba";

        usuario.PasswordHash =
            passwordHasher.HashPassword(
                usuario,
                "PasswordCorrecta123!");

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        var controller =
            CrearController(
                context,
                passwordHasher);

        var request = new LoginRequest
        {
            NombreUsuario =
                "gerente.prueba",

            Password =
                "PasswordIncorrecta123!"
        };

        var resultado =
            await controller.Login(request);

        var unauthorized =
            Assert.IsType<UnauthorizedObjectResult>(
                resultado.Result);

        Assert.Equal(
            "Usuario o contraseña incorrectos.",
            unauthorized.Value);
    }

    [Fact]
    public async Task Login_CredencialesCorrectas_DevuelveToken()
    {
        await using var context = CrearContexto();

        var passwordHasher =
            new PasswordHasher<Usuario>();

        var usuario =
            CrearUsuarioGerente();

        usuario.PasswordHash =
            passwordHasher.HashPassword(
                usuario,
                "Password123!");

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        var controller =
            CrearController(
                context,
                passwordHasher);

        var request = new LoginRequest
        {
            NombreUsuario =
                "gerente",

            Password =
                "Password123!"
        };

        var resultado =
            await controller.Login(request);

        var ok =
            Assert.IsType<OkObjectResult>(
                resultado.Result);

        var respuesta =
            Assert.IsType<LoginResponse>(
                ok.Value);

        Assert.False(
            string.IsNullOrWhiteSpace(
                respuesta.Token));

        Assert.Equal(
            1,
            respuesta.UsuarioId);

        Assert.Equal(
            "gerente",
            respuesta.NombreUsuario);

        Assert.Equal(
            "Gerente de Bodega",
            respuesta.NombreCompleto);

        Assert.Equal(
            "GerenteBodega",
            respuesta.Rol);

        Assert.True(
            respuesta.Expira >
            DateTime.UtcNow);
    }
}