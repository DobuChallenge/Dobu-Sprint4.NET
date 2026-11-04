using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;

namespace Dobu.Infrastructure.Repositories;

public class LogErroRepository(DobuDbContext context) : Repository<LogErro>(context), ILogErroRepository
{
}
