using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IConsultaRepository
{
    IReadOnlyCollection<Consulta> GetAll();
    Consulta? GetById(Guid id);
    void Add(Consulta consulta);
    void Update(Consulta consulta);
    void Delete(Consulta consulta);
    void SaveChanges();
}
