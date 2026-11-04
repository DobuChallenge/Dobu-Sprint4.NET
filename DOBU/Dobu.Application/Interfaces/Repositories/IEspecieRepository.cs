using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IEspecieRepository
{
    IReadOnlyCollection<Especie> GetAll();
    Especie? GetById(Guid id);
    void Add(Especie especie);
    void Update(Especie especie);
    void Delete(Especie especie);
    void SaveChanges();
}
