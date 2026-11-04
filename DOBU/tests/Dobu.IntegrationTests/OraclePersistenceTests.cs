using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Dobu.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dobu.IntegrationTests;

public sealed class OracleFactAttribute : FactAttribute
{
    public OracleFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("DOBU_TEST_ORACLE") is null || Environment.GetEnvironmentVariable("DOBU_TEST_ORACLE_ISOLATED") != "true")
            Skip = "Configure um schema Oracle isolado em DOBU_TEST_ORACLE e confirme DOBU_TEST_ORACLE_ISOLATED=true.";
    }
}

public class OraclePersistenceTests
{
    [OracleFact]
    public async Task Oracle_MigracoesERepositorio_PersistemERevertemRegistroDeTeste()
    {
        // Arrange: este teste exige schema exclusivo, pois aplica as migrações antes da transação.
        var connection = Environment.GetEnvironmentVariable("DOBU_TEST_ORACLE")!;
        var options = new DbContextOptionsBuilder<DobuDbContext>();
        options.UseDobuOracle(connection);
        await using var database = new DobuDbContext(options.Options);
        await database.Database.MigrateAsync();
        await using var transaction = await database.Database.BeginTransactionAsync();
        var repository = new EntityRepository<Especie>(database);
        var species = new Especie("Espécie teste", "Registro temporário de integração Oracle");
        // Act
        repository.Add(species);
        await database.SaveChangesAsync();
        var saved = await repository.FirstOrDefaultAsync(x => x.Id == species.Id);
        var active = await repository.ListAsync(new Dobu.Application.Queries.QueryParameters(), x => x.Id == species.Id && x.Active);
        var migrations = await database.Database.GetAppliedMigrationsAsync();
        await transaction.RollbackAsync();
        database.ChangeTracker.Clear();
        // Assert
        Assert.Equal(species.Id, saved?.Id);
        Assert.Single(active);
        Assert.Contains(migrations, migration => migration.EndsWith("InicialOracleDobu"));
        Assert.False(await repository.AnyAsync(x => x.Id == species.Id));
    }
}
