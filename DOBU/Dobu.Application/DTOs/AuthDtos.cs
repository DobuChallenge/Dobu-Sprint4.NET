namespace Dobu.Application.DTOs;

public record RegisterRequest(string Nome, string Email, string Senha, string TipoUsuario);
public record LoginRequest(string Email, string Senha);
public record AuthResponse(string Token, Guid UsuarioId, string Nome, string Email, string TipoUsuario);
