using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface ILogErroRepository
{
    IReadOnlyCollection<LogErro> GetAll();
    LogErro? GetById(Guid id);
    void Add(LogErro logErro);
    void Update(LogErro logErro);
    void Delete(LogErro logErro);
    void SaveChanges();
}
