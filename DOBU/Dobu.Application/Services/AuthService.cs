using System.IdentityModel.Tokens.Jwt;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Dobu.Application.DTOs;
using Dobu.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Dobu.Application.Services;

public sealed class AuthService(IUserStore users, string signingKey) : IAuthService
{
    private static readonly ActivitySource ActivitySource = new("Dobu.Application");
    private readonly PasswordHasher<Usuario> _hasher = new();

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity("auth.register");
        var email = request.Email.Trim().ToLowerInvariant();
        if (await users.FindByEmailAsync(email, cancellationToken) is not null)
            throw new InvalidOperationException("Já existe usuário cadastrado com esse e-mail.");

        var user = new Usuario(request.Nome, email, request.Senha, request.TipoUsuario);
        user.DefinirSenha(_hasher.HashPassword(user, request.Senha));
        await users.AddAsync(user, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);
        return CreateResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity("auth.login");
        var user = await users.FindByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !SenhaCorresponde(user, request.Senha))
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        return CreateResponse(user);
    }

    private bool SenhaCorresponde(Usuario user, string senha)
    {
        try
        {
            return _hasher.VerifyHashedPassword(user, user.Senha, senha) != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private AuthResponse CreateResponse(Usuario user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.TipoUsuario)
        };
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: credentials);
        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), user.Id, user.Nome, user.Email, user.TipoUsuario);
    }
}
