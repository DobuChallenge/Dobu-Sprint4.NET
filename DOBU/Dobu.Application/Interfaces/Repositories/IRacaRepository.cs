using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IRacaRepository
{
    IReadOnlyCollection<Raca> GetAll();
    Raca? GetById(Guid id);
    void Add(Raca raca);
    void Update(Raca raca);
    void Delete(Raca raca);
    void SaveChanges();
}
