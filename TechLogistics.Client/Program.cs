using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TechLogistics.Client;
using TechLogistics.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
builder.Services.AddScoped<InventarioHistorialApiService>();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<
        JwtAuthorizationMessageHandler>();

    handler.InnerHandler =
        new HttpClientHandler();

    return new HttpClient(handler)
    {
        BaseAddress =
            new Uri("http://localhost:5134/")
    };
});

// Servicios de la aplicación
builder.Services.AddScoped<CatalogoApiService>();
builder.Services.AddScoped<InventarioApiService>();
builder.Services.AddScoped<CentroDistribucionApiService>();
builder.Services.AddScoped<TelemetriaGrpcClientService>();
builder.Services.AddScoped<InventarioClientState>();
builder.Services.AddScoped<InventarioStreamingService>();

// Autenticación
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<
    JwtAuthenticationStateProvider>();

builder.Services.AddScoped<
    AuthenticationStateProvider>(
        sp => sp.GetRequiredService<
            JwtAuthenticationStateProvider>());

// Autorización
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();