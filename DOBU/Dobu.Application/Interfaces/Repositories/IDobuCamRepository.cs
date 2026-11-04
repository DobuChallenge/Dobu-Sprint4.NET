using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IDobuCamRepository
{
    IReadOnlyCollection<DobuCam> GetAll();
    DobuCam? GetById(Guid id);
    void Add(DobuCam dobuCam);
    void Update(DobuCam dobuCam);
    void Delete(DobuCam dobuCam);
    void SaveChanges();
}
