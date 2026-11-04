using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    IReadOnlyCollection<Usuario> GetAll();
    Usuario? GetById(Guid id);
    void Add(Usuario usuario);
    void Update(Usuario usuario);
    void Delete(Usuario usuario);
    void SaveChanges();
}
