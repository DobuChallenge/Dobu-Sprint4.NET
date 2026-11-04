using Dobu.Domain.Commons;

namespace Dobu.Domain.Entities;

public class MensagemAgendamento : BaseEntity
{
    public string Texto { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = "MENSAGEM";
    public Guid AgendamentoId { get; private set; }
    public Agendamento Agendamento { get; private set; } = null!;
    public Guid AutorId { get; private set; }
    public Usuario Autor { get; private set; } = null!;

    private MensagemAgendamento() { }

    public MensagemAgendamento(string texto, string tipo, Guid agendamentoId, Guid autorId)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length is < 1 or > 500) throw new ArgumentException("A mensagem deve ter de 1 a 500 caracteres.");
        if (agendamentoId == Guid.Empty || autorId == Guid.Empty) throw new ArgumentException("Agendamento ou autor inválido");
        Texto = texto.Trim();
        Tipo = string.IsNullOrWhiteSpace(tipo) ? "MENSAGEM" : tipo.Trim().ToUpperInvariant();
        AgendamentoId = agendamentoId;
        AutorId = autorId;
    }
}
