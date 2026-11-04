using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class LogErro : BaseEntity
{
    public string NomeProcedure { get; private set; } = string.Empty;
    public string DescricaoErro { get; private set; } = string.Empty;
    public DateTime DataErro { get; private set; }

    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;

    private LogErro()
    {
    }

    public LogErro(string nomeProcedure, string descricaoErro, DateTime dataErro, Guid usuarioId)
    {
        Atualizar(nomeProcedure, descricaoErro, dataErro, usuarioId);
    }

    public void Atualizar(string nomeProcedure, string descricaoErro, DateTime dataErro, Guid usuarioId)
    {
        NomeProcedure = nomeProcedure;
        DescricaoErro = descricaoErro;
        DataErro = dataErro;
        UsuarioId = usuarioId;
    }
}
