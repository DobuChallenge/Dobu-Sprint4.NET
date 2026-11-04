using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;

namespace Dobu.Infrastructure.Repositories;

public class ConsultaRepository(DobuDbContext context) : Repository<Consulta>(context), IConsultaRepository
{
}
