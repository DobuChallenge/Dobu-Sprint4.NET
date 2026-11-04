using Dobu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Infrastructure.Persistence;

public class DobuDbContext(DbContextOptions<DobuDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Especie> Especies { get; set; }
    public DbSet<Raca> Racas { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }
    public DbSet<Prontuario> Prontuarios { get; set; }
    public DbSet<Vacina> Vacinas { get; set; }
    public DbSet<Pagamento> Pagamentos { get; set; }
    public DbSet<Lembrete> Lembretes { get; set; }
    public DbSet<DobuCam> DobuCams { get; set; }
    public DbSet<AnaliseIa> AnalisesIa { get; set; }
    public DbSet<LogErro> LogsErro { get; set; }
    public DbSet<InformacaoCuidado> InformacoesCuidado { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DobuDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
