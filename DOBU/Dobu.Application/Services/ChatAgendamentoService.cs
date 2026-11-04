using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Dobu.Domain.Entities;

namespace Dobu.Application.Services;

public record ChatMessageRequest(string Texto, string? Tipo);
public record ChatMessageResponse(Guid Id, string Texto, string Tipo, Guid AutorId, string AutorNome, string AutorTipo, DateTime CreatedAt);

public interface IChatAgendamentoService
{
    Task<IReadOnlyList<ChatMessageResponse>> ListAsync(Guid appointmentId, Guid userId, CancellationToken cancellationToken);
    Task<ChatMessageResponse> SendAsync(Guid appointmentId, Guid userId, ChatMessageRequest request, CancellationToken cancellationToken);
}

public sealed class ChatAgendamentoService(IDobuUnitOfWork work) : IChatAgendamentoService
{
    private async Task<Agendamento> RequireAppointment(Guid id, Guid userId, CancellationToken cancellationToken) =>
        await work.Agendamentos.FirstOrDefaultAsync(x => x.Id == id && (x.VeterinarioId == userId || x.Pet.ResponsavelId == userId),
            cancellationToken, includes: [x => x.Pet]) ?? throw new KeyNotFoundException("Agendamento não encontrado ou sem permissão.");

    public async Task<IReadOnlyList<ChatMessageResponse>> ListAsync(Guid appointmentId, Guid userId, CancellationToken cancellationToken)
    {
        await RequireAppointment(appointmentId, userId, cancellationToken);
        var messages = await work.MensagensAgendamento.ListAsync(new QueryParameters(),
            x => x.AgendamentoId == appointmentId && x.Active, [x => x.Autor], cancellationToken);
        return messages.Select(message => Response(message, message.Autor)).ToList();
    }

    public async Task<ChatMessageResponse> SendAsync(Guid appointmentId, Guid userId, ChatMessageRequest request, CancellationToken cancellationToken)
    {
        var appointment = await RequireAppointment(appointmentId, userId, cancellationToken);
        var author = await work.Usuarios.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Usuário não encontrado.");
        var confirmation = string.Equals(request.Tipo, "CONFIRMACAO", StringComparison.OrdinalIgnoreCase);
        var type = confirmation ? "CONFIRMACAO" : "MENSAGEM";
        if (confirmation)
        {
            var previous = await work.MensagensAgendamento.FirstOrDefaultAsync(x => x.AgendamentoId == appointmentId && x.AutorId == userId && x.Tipo == type, cancellationToken);
            if (previous is not null) return Response(previous, author);
        }
        var message = new MensagemAgendamento(confirmation ? "Confirmo que posso comparecer a este atendimento." : request.Texto, type, appointmentId, userId);
        work.MensagensAgendamento.Add(message);
        if (confirmation && await work.MensagensAgendamento.AnyAsync(x => x.AgendamentoId == appointmentId && x.Tipo == type && x.AutorId != userId, cancellationToken))
            appointment.Atualizar(appointment.DataAgendamento, "CONFIRMADO", appointment.PetId, appointment.VeterinarioId);
        await work.SaveChangesAsync(cancellationToken);
        return Response(message, author);
    }

    private static ChatMessageResponse Response(MensagemAgendamento message, Usuario author) =>
        new(message.Id, message.Texto, message.Tipo, author.Id, author.Nome, author.TipoUsuario, message.CreatedAt);
}
