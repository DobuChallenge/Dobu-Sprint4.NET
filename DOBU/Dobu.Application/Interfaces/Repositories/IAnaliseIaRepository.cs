using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IAnaliseIaRepository
{
    IReadOnlyCollection<AnaliseIa> GetAll();
    AnaliseIa? GetById(Guid id);
    void Add(AnaliseIa analiseIa);
    void Update(AnaliseIa analiseIa);
    void Delete(AnaliseIa analiseIa);
    void SaveChanges();
}
