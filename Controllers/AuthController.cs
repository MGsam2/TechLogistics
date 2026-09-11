using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechLogistics.Data;
using TechLogistics.DTOs;
using TechLogistics.Models;
using TechLogistics.Services;

namespace TechLogistics.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TechLogisticsDbContext context;
    private readonly JwtService jwtService;
    private readonly IPasswordHasher<Usuario> passwordHasher;
    private readonly ILogger<AuthController> logger;

    public AuthController(
        TechLogisticsDbContext context,
        JwtService jwtService,
        IPasswordHasher<Usuario> passwordHasher,
        ILogger<AuthController> logger)
    {
        this.context = context;
        this.jwtService = jwtService;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        var usuario = await context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(
                u => u.NombreUsuario == request.NombreUsuario);

        if (usuario is null || !usuario.Activo)
        {
            return Unauthorized(
                "Usuario o contraseña incorrectos.");
        }

        var resultado = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            request.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized(
                "Usuario o contraseña incorrectos.");
        }

        if (usuario.Rol is null)
        {
            return Unauthorized(
                "El usuario no tiene un rol asignado.");
        }

        var (token, expira) =
            jwtService.GenerarToken(usuario);

        logger.LogInformation(
            "Inicio de sesión exitoso para {Usuario}",
            usuario.NombreUsuario);

        return Ok(new LoginResponse
        {
            Token = token,
            UsuarioId = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Rol = usuario.Rol.Nombre,
            Expira = expira
        });
    }
}