using Dobu.Application.Interfaces.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dobu.Api.Health;

public sealed class MongoHealthCheck(ICameraObservationRepository observations) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try { await observations.PingAsync(cancellationToken); return HealthCheckResult.Healthy("MongoDB acessível."); }
        catch (Exception exception) { return HealthCheckResult.Unhealthy("MongoDB indisponível.", exception); }
    }
}
