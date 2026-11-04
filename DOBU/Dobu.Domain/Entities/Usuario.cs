using Dobu.Domain.Commons;
using Dobu.Domain.Enums;
using System.Text.Json.Serialization;

namespace Dobu.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    [JsonIgnore]
    public string Senha { get; private set; } = string.Empty;
    public string TipoUsuario { get; private set; } = string.Empty;

    public List<Pet> PetsResponsavel { get; private set; } = new();
    public List<Consulta> ConsultasVeterinario { get; private set; } = new();
    public List<Agendamento> AgendamentosVeterinario { get; private set; } = new();
    public List<LogErro> LogsErro { get; private set; } = new();

    private Usuario()
    {
    }

    public Usuario(string nome, string email, string senha, string tipoUsuario)
    {
        Atualizar(nome, email, senha, tipoUsuario);
    }

    public void Atualizar(string nome, string email, string senha, string tipoUsuario)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new ArgumentException("Nome invalido");

        if (string.IsNullOrWhiteSpace(email) || email.Length < 5 || !email.Contains('@'))
            throw new ArgumentException("Email invalido");

        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
            throw new ArgumentException("Senha invalida");

        if (!Enum.GetNames<TipoUsuario>().Any(x => x.Equals(tipoUsuario, StringComparison.OrdinalIgnoreCase)) ||
            !Enum.TryParse<TipoUsuario>(tipoUsuario, ignoreCase: true, out var tipoUsuarioValido))
            throw new ArgumentException("Tipo de usuario invalido");

        Nome = nome;
        Email = email;
        Senha = senha;
        TipoUsuario = tipoUsuarioValido.ToString().ToUpperInvariant();
    }

    public void DefinirSenha(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("Hash de senha inválido");

        Senha = senhaHash;
    }
}
