using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;

namespace Dobu.Infrastructure.Repositories;

public class PetRepository(DobuDbContext context) : Repository<Pet>(context), IPetRepository
{
}
