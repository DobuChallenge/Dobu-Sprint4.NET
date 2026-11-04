using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class InformacaoCuidado : BaseEntity
{
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public Guid PetId { get; private set; }
    public Pet Pet { get; private set; } = null!;

    private InformacaoCuidado() { }

    public InformacaoCuidado(string titulo, string descricao, Guid petId) => Atualizar(titulo, descricao, petId);

    public void Atualizar(string titulo, string descricao, Guid petId)
    {
        if (string.IsNullOrWhiteSpace(titulo) || titulo.Length < 2) throw new ArgumentException("Título inválido");
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Length < 5) throw new ArgumentException("Descrição inválida");
        if (petId == Guid.Empty) throw new ArgumentException("Pet inválido");
        Titulo = titulo.Trim();
        Descricao = descricao.Trim();
        PetId = petId;
    }
}
