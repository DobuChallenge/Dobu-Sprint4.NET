using Dobu.Application.Services;
using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Infrastructure.Repositories;

public sealed class UserStore(DobuDbContext context) : IUserStore
{
    public Task<Usuario?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        context.Usuarios.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default) =>
        await context.Usuarios.AddAsync(usuario, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}
