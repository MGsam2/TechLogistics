using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TechLogistics.Tests.Integration;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme =
                        TestAuthenticationHandler.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        TestAuthenticationHandler.AuthenticationScheme;
                })
                .AddScheme<
                    AuthenticationSchemeOptions,
                    TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationScheme,
                    _ =>
                    {
                    });
        });
    }
}

public class TestAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.Name,
                "usuario-integracion"),

            new Claim(
                ClaimTypes.Role,
                "GerenteBodega")
        };

        var identity = new ClaimsIdentity(
            claims,
            AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            AuthenticationScheme);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}