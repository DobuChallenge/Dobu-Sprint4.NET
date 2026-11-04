using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;

namespace Dobu.Infrastructure.Repositories;

public sealed class DobuUnitOfWork(DobuDbContext context) : IDobuUnitOfWork
{
    public IEntityRepository<Usuario> Usuarios { get; } = new EntityRepository<Usuario>(context);
    public IEntityRepository<Especie> Especies { get; } = new EntityRepository<Especie>(context);
    public IEntityRepository<Raca> Racas { get; } = new EntityRepository<Raca>(context);
    public IEntityRepository<Pet> Pets { get; } = new EntityRepository<Pet>(context);
    public IEntityRepository<Consulta> Consultas { get; } = new EntityRepository<Consulta>(context);
    public IEntityRepository<Agendamento> Agendamentos { get; } = new EntityRepository<Agendamento>(context);
    public IEntityRepository<Prontuario> Prontuarios { get; } = new EntityRepository<Prontuario>(context);
    public IEntityRepository<Vacina> Vacinas { get; } = new EntityRepository<Vacina>(context);
    public IEntityRepository<Pagamento> Pagamentos { get; } = new EntityRepository<Pagamento>(context);
    public IEntityRepository<Lembrete> Lembretes { get; } = new EntityRepository<Lembrete>(context);
    public IEntityRepository<DobuCam> DobuCams { get; } = new EntityRepository<DobuCam>(context);
    public IEntityRepository<AnaliseIa> AnalisesIa { get; } = new EntityRepository<AnaliseIa>(context);
    public IEntityRepository<LogErro> LogsErro { get; } = new EntityRepository<LogErro>(context);
    public IEntityRepository<InformacaoCuidado> InformacoesCuidado { get; } = new EntityRepository<InformacaoCuidado>(context);
    public IEntityRepository<MensagemAgendamento> MensagensAgendamento { get; } = new EntityRepository<MensagemAgendamento>(context);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}
