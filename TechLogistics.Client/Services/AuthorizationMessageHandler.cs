using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace TechLogistics.Client.Services;

public class AuthorizationMessageHandler
    : DelegatingHandler
{
    private readonly IJSRuntime jsRuntime;

    public AuthorizationMessageHandler(
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
            var token = await jsRuntime
                .InvokeAsync<string?>(
                    "localStorage.getItem",
                    "techlogistics_token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }
        }
        catch
        {
            // Si no existe token, la petición continúa
            // sin autenticación.
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}