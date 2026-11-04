using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dobu.Api.Health;

public sealed class ExternalServiceHealthCheck(IHttpClientFactory clients, IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var url = configuration["Observability:ExternalServiceUrl"];
        if (!Uri.TryCreate(url, UriKind.Absolute, out var serviceUri) ||
            (serviceUri.Scheme != Uri.UriSchemeHttp && serviceUri.Scheme != Uri.UriSchemeHttps))
        {
            return HealthCheckResult.Unhealthy("URL do serviço externo não configurada ou inválida.");
        }

        try
        {
            using var response = await clients
                .CreateClient("ExternalServiceHealthCheck")
                .GetAsync(serviceUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"Serviço externo respondeu {(int)response.StatusCode}.")
                : HealthCheckResult.Unhealthy($"Serviço externo respondeu {(int)response.StatusCode}.");
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("Tempo limite do serviço externo excedido.", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Serviço externo indisponível.", ex);
        }
    }
}
