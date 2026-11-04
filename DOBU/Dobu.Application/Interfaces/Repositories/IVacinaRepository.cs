using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IVacinaRepository
{
    IReadOnlyCollection<Vacina> GetAll();
    Vacina? GetById(Guid id);
    void Add(Vacina vacina);
    void Update(Vacina vacina);
    void Delete(Vacina vacina);
    void SaveChanges();
}
