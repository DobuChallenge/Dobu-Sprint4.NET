using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;

namespace Dobu.Infrastructure.Repositories;

public class EspecieRepository(DobuDbContext context) : Repository<Especie>(context), IEspecieRepository
{
}
