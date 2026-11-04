using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Dobu.Infrastructure.Persistence;

namespace Dobu.IntegrationTests;

public sealed class DobuApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(), $"dobu-tests-{Guid.NewGuid():N}.db");
    private readonly string _mongoDatabase = $"dobu_tests_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "Sqlite",
                ["Database:ApplyMigrations"] = "false",
                ["Database:UseTestSchema"] = "true",
                ["Mongo:Enabled"] = (Environment.GetEnvironmentVariable("DOBU_TEST_MONGO_URL") is not null).ToString(),
                ["Mongo:ConnectionString"] = Environment.GetEnvironmentVariable("DOBU_TEST_MONGO_URL") ?? "mongodb://localhost:27017",
                ["Mongo:Database"] = _mongoDatabase,
                ["ConnectionStrings:DobuSqlite"] = $"Data Source={_databasePath}",
                ["Observability:ExternalServiceUrl"] = "https://external-service.test/health"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<DobuDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<DobuDbContext>>();
            services.AddDbContext<DobuDbContext>(options => options.UseSqlite($"Data Source={_databasePath}"));
            services.AddHttpClient("ExternalServiceHealthCheck")
                .ConfigurePrimaryHttpMessageHandler(() => new HealthyExternalServiceHandler());
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && Environment.GetEnvironmentVariable("DOBU_TEST_MONGO_URL") is string mongoUrl)
            new MongoDB.Driver.MongoClient(mongoUrl).DropDatabase(_mongoDatabase);

        if (disposing && File.Exists(_databasePath))
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(_databasePath);
        }
    }

    private sealed class HealthyExternalServiceHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
