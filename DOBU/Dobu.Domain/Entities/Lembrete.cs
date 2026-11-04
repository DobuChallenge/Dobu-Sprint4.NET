using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Lembrete : BaseEntity
{
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataLembrete { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public Guid PetId { get; private set; }
    public Pet Pet { get; private set; } = null!;

    private Lembrete()
    {
    }

    public Lembrete(string descricao, DateTime dataLembrete, string status, Guid petId)
    {
        Atualizar(descricao, dataLembrete, status, petId);
    }

    public void Atualizar(string descricao, DateTime dataLembrete, string status, Guid petId)
    {
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Length < 5)
            throw new ArgumentException("Descricao do lembrete invalida");

        Descricao = descricao;
        DataLembrete = dataLembrete;
        Status = status;
        PetId = petId;
    }
}
