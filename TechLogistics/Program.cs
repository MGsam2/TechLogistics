using TechLogistics.Components;
using TechLogistics.Services;
using Microsoft.EntityFrameworkCore;
using TechLogistics.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// CONFIGURACIÓN JWT
// ============================================================

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "No se encontró Jwt:Key en la configuración.");
}

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ValidateIssuer = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidateAudience = true,

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

// ============================================================
// CORS PARA EL CLIENTE BLAZOR WEBASSEMBLY
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClienteWebAssembly", policy =>
    {
        policy
            .WithOrigins("http://localhost:5085")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================================================
// gRPC
// ============================================================

builder.Services.AddGrpc();

// ============================================================
// SERVICIOS DE LA APLICACIÓN
// ============================================================

builder.Services.AddScoped<CentroDistribucionService>();
builder.Services.AddHostedService<InventarioSimulatorService>();
builder.Services.AddScoped<InventarioService>();
builder.Services.AddSingleton<InventarioNotifier>();
builder.Services.AddScoped<InventarioState>();
builder.Services.AddScoped<PreferenciasUsuarioService>();

// ============================================================
// SERVICIOS DE AUTENTICACIÓN
// ============================================================

builder.Services.AddScoped<JwtService>();

builder.Services.AddScoped<
    IPasswordHasher<TechLogistics.Models.Usuario>,
    PasswordHasher<TechLogistics.Models.Usuario>>();

// ============================================================
// API CONTROLLERS
// ============================================================

builder.Services.AddControllers();

// ============================================================
// BLAZOR
// ============================================================

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// ============================================================
// BASE DE DATOS
// ============================================================

builder.Services.AddDbContext<TechLogisticsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "TechLogisticsDb")));

// ============================================================
// CONSTRUIR APLICACIÓN
// ============================================================

var app = builder.Build();

// ============================================================
// CORS
// ============================================================

app.UseCors("ClienteWebAssembly");
app.UseGrpcWeb();

// ============================================================
// CONFIGURACIÓN DEL PIPELINE
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

// ============================================================
// MANEJO DE ERRORES PARA RUTAS NO API
// ============================================================

app.UseWhen(
    context =>
        !context.Request.Path.StartsWithSegments("/api"),

    appBuilder =>
    {
        appBuilder.UseStatusCodePagesWithReExecute(
            "/not-found",
            createScopeForStatusCodePages: true);
    });

// ============================================================
// HTTPS
// ============================================================

app.UseHttpsRedirection();

// ============================================================
// AUTENTICACIÓN Y AUTORIZACIÓN
// ============================================================

app.UseAuthentication();

app.UseAuthorization();

// ============================================================
// ANTIFORGERY
// ============================================================

app.UseAntiforgery();

// ============================================================
// ARCHIVOS ESTÁTICOS
// ============================================================

app.MapStaticAssets();

// ============================================================
// API
// ============================================================

app.MapControllers();

// ============================================================
// GRPC
// ============================================================
app.MapGrpcService<TelemetriaGrpcService>().EnableGrpcWeb();

// ============================================================
// BLAZOR
// ============================================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(TechLogistics.Client._Imports).Assembly);

await DbInitializer.InicializarAsync(app.Services);

app.Run();