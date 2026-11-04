using Dobu.Domain.Entities;

namespace Dobu.Application.Interfaces.Repositories;

public interface IPagamentoRepository
{
    IReadOnlyCollection<Pagamento> GetAll();
    Pagamento? GetById(Guid id);
    void Add(Pagamento pagamento);
    void Update(Pagamento pagamento);
    void Delete(Pagamento pagamento);
    void SaveChanges();
}
