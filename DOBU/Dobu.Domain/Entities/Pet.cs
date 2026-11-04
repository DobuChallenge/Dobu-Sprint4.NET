using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Pet : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public int Idade { get; private set; }

    public Guid RacaId { get; private set; }
    public Raca Raca { get; private set; } = null!;

    public Guid ResponsavelId { get; private set; }
    public Usuario Responsavel { get; private set; } = null!;

    public List<Consulta> Consultas { get; private set; } = new();
    public List<Agendamento> Agendamentos { get; private set; } = new();
    public List<Vacina> Vacinas { get; private set; } = new();
    public List<Lembrete> Lembretes { get; private set; } = new();
    public List<DobuCam> DobuCams { get; private set; } = new();
    public List<InformacaoCuidado> InformacoesCuidado { get; private set; } = new();

    private Pet()
    {
    }

    public Pet(string nome, int idade, Guid racaId, Guid responsavelId)
    {
        Atualizar(nome, idade, racaId, responsavelId);
    }

    public void Atualizar(string nome, int idade, Guid racaId, Guid responsavelId)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new ArgumentException("Nome do pet invalido");

        if (idade < 0)
            throw new ArgumentException("Idade invalida");

        Nome = nome;
        Idade = idade;
        RacaId = racaId;
        ResponsavelId = responsavelId;
    }
}
