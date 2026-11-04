using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class AnaliseIa : BaseEntity
{
    public string Descricao { get; private set; } = string.Empty;
    public int Risco { get; private set; }
    public DateTime DataAnalise { get; private set; }

    public Guid ProntuarioId { get; private set; }
    public Prontuario Prontuario { get; private set; } = null!;

    private AnaliseIa()
    {
    }

    public AnaliseIa(string descricao, int risco, DateTime dataAnalise, Guid prontuarioId)
    {
        Atualizar(descricao, risco, dataAnalise, prontuarioId);
    }

    public void Atualizar(string descricao, int risco, DateTime dataAnalise, Guid prontuarioId)
    {
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Length < 5)
            throw new ArgumentException("Descricao da analise invalida");

        if (risco < 0 || risco > 10)
            throw new ArgumentException("Risco deve estar entre 0 e 10");

        Descricao = descricao;
        Risco = risco;
        DataAnalise = dataAnalise;
        ProntuarioId = prontuarioId;
    }
}
