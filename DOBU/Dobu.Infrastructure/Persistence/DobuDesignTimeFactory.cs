using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dobu.Infrastructure.Persistence;

public sealed class DobuDesignTimeFactory : IDesignTimeDbContextFactory<DobuDbContext>
{
    public DobuDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__DobuOracle")
            ?? "User Id=design;Password=design;Data Source=localhost:1521/FREEPDB1";
        return new DobuDbContext(new DbContextOptionsBuilder<DobuDbContext>().UseOracle(connection).Options);
    }
}
