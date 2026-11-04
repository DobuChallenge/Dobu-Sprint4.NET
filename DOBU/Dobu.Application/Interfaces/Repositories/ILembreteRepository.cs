using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface ILembreteRepository
{
    IReadOnlyCollection<Lembrete> GetAll();
    Lembrete? GetById(Guid id);
    void Add(Lembrete lembrete);
    void Update(Lembrete lembrete);
    void Delete(Lembrete lembrete);
    void SaveChanges();
}
