using Dobu.Application.DTOs;
using Dobu.Domain.Entities;

namespace Dobu.Application.Services;

public interface IUserStore
{
    Task<Usuario?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
