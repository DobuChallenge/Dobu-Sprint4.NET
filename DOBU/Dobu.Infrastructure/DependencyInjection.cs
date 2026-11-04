using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Services;
using Dobu.Infrastructure.Persistence;
using Dobu.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dobu.Infrastructure.NoSql;
using MongoDB.Driver;

namespace Dobu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DobuDbContext>((provider, options) =>
        {
            var settings = provider.GetRequiredService<IConfiguration>();
            if (!string.Equals(settings["Database:Provider"] ?? "Oracle", "Oracle", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("A API usa Oracle. SQLite é restrito às bases temporárias dos testes.");
            var oracleConn = settings.GetConnectionString("DobuOracle");
            if (string.IsNullOrWhiteSpace(oracleConn))
                throw new InvalidOperationException("Configure ConnectionStrings:DobuOracle nas configuracoes seguras do ambiente.");
            options.UseDobuOracle(oracleConn);
        });

        services.AddScoped<IDobuUnitOfWork, DobuUnitOfWork>();
        services.AddScoped<IChatAgendamentoService, ChatAgendamentoService>();
        services.AddScoped<CameraObservationService>();
        services.AddScoped<PetService>();
        services.AddScoped<AgendamentoService>();
        services.AddSingleton<IMongoClient>(_ =>
        {
            var connection = configuration["Mongo:ConnectionString"] ?? "mongodb://localhost:27017";
            var settings = MongoClientSettings.FromConnectionString(connection);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);
            return new MongoClient(settings);
        });
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(configuration["Mongo:Database"] ?? "dobu"));
        services.AddScoped<ICameraObservationRepository, MongoCameraObservationRepository>();
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IAnaliseIaRepository, AnaliseIaRepository>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IDobuCamRepository, DobuCamRepository>();
        services.AddScoped<IEspecieRepository, EspecieRepository>();
        services.AddScoped<ILembreteRepository, LembreteRepository>();
        services.AddScoped<ILogErroRepository, LogErroRepository>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IProntuarioRepository, ProntuarioRepository>();
        services.AddScoped<IRacaRepository, RacaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUserStore, UserStore>();
        services.AddScoped<IVacinaRepository, VacinaRepository>();

        return services;
    }
}
