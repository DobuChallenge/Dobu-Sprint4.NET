using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IPetRepository
{
    IReadOnlyCollection<Pet> GetAll();
    Pet? GetById(Guid id);
    void Add(Pet pet);
    void Update(Pet pet);
    void Delete(Pet pet);
    void SaveChanges();
}
