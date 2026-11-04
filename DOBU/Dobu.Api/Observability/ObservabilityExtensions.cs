using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Dobu.Api.Observability;

public static class ObservabilityExtensions
{
    public const string MeterName = "Dobu.Api";

    public static IServiceCollection AddDobuObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("Dobu.Api"))
            .WithTracing(tracing => tracing.AddSource("Dobu.Application").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddConsoleExporter())
            .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddMeter(ObservabilityExtensions.MeterName).AddPrometheusExporter());
        return services;
    }
}
