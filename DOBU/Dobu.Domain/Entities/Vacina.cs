using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Vacina : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public DateTime DataAplicacao { get; private set; }
    public DateTime? DataProximaDose { get; private set; }

    public Guid PetId { get; private set; }
    public Pet Pet { get; private set; } = null!;

    private Vacina()
    {
    }

    public Vacina(string nome, DateTime dataAplicacao, DateTime? dataProximaDose, Guid petId)
    {
        Atualizar(nome, dataAplicacao, dataProximaDose, petId);
    }

    public void Atualizar(string nome, DateTime dataAplicacao, DateTime? dataProximaDose, Guid petId)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length < 2)
            throw new ArgumentException("Nome da vacina invalido");

        Nome = nome;
        DataAplicacao = dataAplicacao;
        DataProximaDose = dataProximaDose;
        PetId = petId;
    }
}
