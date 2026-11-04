using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore.Infrastructure;

namespace Dobu.Infrastructure.Persistence;

public static class OracleDatabase
{
    public const string MigrationsAssembly = "Dobu.Migrations.Oracle";
    public const string HistoryTable = "DOBU_EF_MIGRATIONS";

    public static DbContextOptionsBuilder UseDobuOracle(this DbContextOptionsBuilder options, string connection) =>
        options.UseOracle(connection, oracle => oracle.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)
            .MigrationsAssembly(MigrationsAssembly).MigrationsHistoryTable(HistoryTable));
}
