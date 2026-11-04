using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class Agendamento : BaseEntity
{
    public DateTime DataAgendamento { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public Guid PetId { get; private set; }
    public Pet Pet { get; private set; } = null!;

    public Guid VeterinarioId { get; private set; }
    public Usuario Veterinario { get; private set; } = null!;

    private Agendamento()
    {
    }

    public Agendamento(DateTime dataAgendamento, string status, Guid petId, Guid veterinarioId)
    {
        Atualizar(dataAgendamento, status, petId, veterinarioId);
    }

    public void Atualizar(DateTime dataAgendamento, string status, Guid petId, Guid veterinarioId)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status do agendamento invalido");

        DataAgendamento = dataAgendamento;
        Status = status;
        PetId = petId;
        VeterinarioId = veterinarioId;
    }
}
