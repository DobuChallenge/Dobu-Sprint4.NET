using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;

namespace Dobu.Infrastructure.Repositories;

public class AgendamentoRepository(DobuDbContext context) : Repository<Agendamento>(context), IAgendamentoRepository
{
}
