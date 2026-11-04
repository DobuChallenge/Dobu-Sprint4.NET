using System.Text;
using System.Text.Json.Serialization;
using Dobu.Api.Extensions;
using Dobu.Api.Health;
using Dobu.Api.Middleware;
using Dobu.Api.Observability;
using Dobu.Application.Services;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var jwtSigningKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtSigningKey) || jwtSigningKey.Length < 32)
    throw new InvalidOperationException("Configure Jwt:Key com pelo menos 32 caracteres nas configuracoes seguras do ambiente.");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/dobu-.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => options.AddPolicy("Mobile", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Link", "X-Total-Count")));
builder.Services.AddControllers(options => options.Filters.Add<Dobu.Api.Filters.HypermediaFilter>()).AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddScoped<IAuthService>(services =>
    new AuthService(services.GetRequiredService<IUserStore>(), jwtSigningKey));

var jwtKey = Encoding.UTF8.GetBytes(jwtSigningKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = false, ValidateAudience = false, ValidateLifetime = true
    });
builder.Services.AddAuthorization(options => options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("ExternalServiceHealthCheck", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DobuDbContext>("database", tags: ["ready"])
    .AddCheck<ExternalServiceHealthCheck>("external-service", tags: ["ready"]);
if (builder.Configuration.GetValue<bool>("Mongo:Enabled")) builder.Services.AddHealthChecks().AddCheck<MongoHealthCheck>("mongodb", tags: ["ready"]);
builder.Services.AddDobuObservability();

var app = builder.Build();
if (app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Database:UseTestSchema"))
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<DobuDbContext>();
    if (!database.Database.IsSqlite()) throw new InvalidOperationException("O schema temporário é exclusivo dos testes SQLite.");
    database.Database.EnsureCreated();
}
if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var migrationScope = app.Services.CreateScope();
    await migrationScope.ServiceProvider.GetRequiredService<DobuDbContext>().Database.MigrateAsync();
}
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
});
app.UseMiddleware<RequestMetricsMiddleware>();
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dobu API v1"); options.RoutePrefix = "swagger"; });
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseCors("Mobile");
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.ToDictionary(entry => entry.Key, entry => new { status = entry.Value.Status.ToString(), description = entry.Value.Description })
        };
        await context.Response.WriteAsJsonAsync(payload);
    }
}).AllowAnonymous();
app.MapPrometheusScrapingEndpoint("/metrics").AllowAnonymous();
app.MapControllers();
app.Run();

public partial class Program { }
