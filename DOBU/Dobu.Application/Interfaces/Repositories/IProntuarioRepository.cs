using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IProntuarioRepository
{
    IReadOnlyCollection<Prontuario> GetAll();
    Prontuario? GetById(Guid id);
    void Add(Prontuario prontuario);
    void Update(Prontuario prontuario);
    void Delete(Prontuario prontuario);
    void SaveChanges();
}
