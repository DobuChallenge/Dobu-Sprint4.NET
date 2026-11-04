using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Prontuario : BaseEntity
{
    public string Diagnostico { get; private set; } = string.Empty;
    public string Observacoes { get; private set; } = string.Empty;

    public Guid ConsultaId { get; private set; }
    public Consulta Consulta { get; private set; } = null!;

    public AnaliseIa? AnaliseIa { get; private set; }

    private Prontuario()
    {
    }

    public Prontuario(string diagnostico, string observacoes, Guid consultaId)
    {
        Atualizar(diagnostico, observacoes, consultaId);
    }

    public void Atualizar(string diagnostico, string observacoes, Guid consultaId)
    {
        if (string.IsNullOrWhiteSpace(diagnostico) || diagnostico.Length < 5)
            throw new ArgumentException("Diagnostico invalido");

        Diagnostico = diagnostico;
        Observacoes = observacoes;
        ConsultaId = consultaId;
    }
}
