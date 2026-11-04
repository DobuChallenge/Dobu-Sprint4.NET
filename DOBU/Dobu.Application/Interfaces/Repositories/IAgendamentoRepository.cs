using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IAgendamentoRepository
{
    IReadOnlyCollection<Agendamento> GetAll();
    Agendamento? GetById(Guid id);
    void Add(Agendamento agendamento);
    void Update(Agendamento agendamento);
    void Delete(Agendamento agendamento);
    void SaveChanges();
}
