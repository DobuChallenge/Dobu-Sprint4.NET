using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;

namespace Dobu.Infrastructure.Repositories;

public class PagamentoRepository(DobuDbContext context) : Repository<Pagamento>(context), IPagamentoRepository
{
}
