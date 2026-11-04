using System.Diagnostics;
using System.Diagnostics.Metrics;
using Dobu.Api.Observability;

namespace Dobu.Api.Middleware;

public sealed class RequestMetricsMiddleware(RequestDelegate next)
{
    private static readonly Meter Meter = new(ObservabilityExtensions.MeterName);
    private static readonly Counter<long> Errors = Meter.CreateCounter<long>("dobu_api_errors_total", description: "Total de respostas HTTP de erro");
    private static readonly Histogram<double> Duration = Meter.CreateHistogram<double>("dobu_api_response_duration_ms", unit: "ms", description: "Duração das respostas HTTP");

    public async Task InvokeAsync(HttpContext context)
    {
        var start = Stopwatch.GetTimestamp();
        try
        {
            await next(context);
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            Record(context, start);
            throw;
        }

        Record(context, start);
    }

    private static void Record(HttpContext context, long start)
    {
        var elapsed = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
        Duration.Record(elapsed, new KeyValuePair<string, object?>("http.status_code", context.Response.StatusCode));
        if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
            Errors.Add(1, new KeyValuePair<string, object?>("http.status_code", context.Response.StatusCode));
    }
}
