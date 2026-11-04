using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dobu.Infrastructure.Persistence;

public sealed class DobuDesignTimeFactory : IDesignTimeDbContextFactory<DobuDbContext>
{
    public DobuDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets("dobu-sprint4-net").Build();
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__DobuOracle") ?? configuration.GetConnectionString("DobuOracle")
            ?? "User Id=design;Password=design;Data Source=localhost:1521/FREEPDB1";
        var options = new DbContextOptionsBuilder<DobuDbContext>();
        options.UseDobuOracle(connection);
        return new DobuDbContext(options.Options);
    }
}
