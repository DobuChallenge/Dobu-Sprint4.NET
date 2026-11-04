using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class DobuCam : BaseEntity
{
    public string Localizacao { get; private set; } = string.Empty;
    public string StatusCamera { get; private set; } = string.Empty;
    public DateTime? DataUltimaMovimentacao { get; private set; }

    public Guid PetId { get; private set; }
    public Pet Pet { get; private set; } = null!;

    private DobuCam()
    {
    }

    public DobuCam(string localizacao, string statusCamera, DateTime? dataUltimaMovimentacao, Guid petId)
    {
        Atualizar(localizacao, statusCamera, dataUltimaMovimentacao, petId);
    }

    public void Atualizar(string localizacao, string statusCamera, DateTime? dataUltimaMovimentacao, Guid petId)
    {
        if (string.IsNullOrWhiteSpace(localizacao))
            throw new ArgumentException("Localizacao invalida");

        Localizacao = localizacao;
        StatusCamera = statusCamera;
        DataUltimaMovimentacao = dataUltimaMovimentacao;
        PetId = petId;
    }
}
