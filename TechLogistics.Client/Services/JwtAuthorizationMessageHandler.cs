using System.Net;
using Microsoft.JSInterop;

namespace TechLogistics.Client.Services;

public class JwtAuthorizationMessageHandler
    : DelegatingHandler
{
    private readonly IJSRuntime jsRuntime;

    public JwtAuthorizationMessageHandler(
        IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage>
        SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
    {
        try
        {
            var token =
                await jsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    "techlogistics_token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }
        }
        catch
        {
            // Si no existe el token, la petición continúa
            // sin autenticación y la API responderá 401.
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}