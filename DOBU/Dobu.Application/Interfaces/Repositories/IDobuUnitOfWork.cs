using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IDobuUnitOfWork
{
    IEntityRepository<Usuario> Usuarios { get; }
    IEntityRepository<Especie> Especies { get; }
    IEntityRepository<Raca> Racas { get; }
    IEntityRepository<Pet> Pets { get; }
    IEntityRepository<Consulta> Consultas { get; }
    IEntityRepository<Agendamento> Agendamentos { get; }
    IEntityRepository<Prontuario> Prontuarios { get; }
    IEntityRepository<Vacina> Vacinas { get; }
    IEntityRepository<Pagamento> Pagamentos { get; }
    IEntityRepository<Lembrete> Lembretes { get; }
    IEntityRepository<DobuCam> DobuCams { get; }
    IEntityRepository<AnaliseIa> AnalisesIa { get; }
    IEntityRepository<LogErro> LogsErro { get; }
    IEntityRepository<InformacaoCuidado> InformacoesCuidado { get; }
    IEntityRepository<MensagemAgendamento> MensagensAgendamento { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
