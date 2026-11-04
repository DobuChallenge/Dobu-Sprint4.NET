using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;

namespace Dobu.Application.Services;

public record AgendamentoRequest(DateTime DataAgendamento, string Status, Guid PetId, Guid VeterinarioId);

public sealed class AgendamentoService(IDobuUnitOfWork work)
{
    private async Task ValidateReferences(AgendamentoRequest request, CancellationToken cancellationToken)
    {
        if (!await work.Pets.AnyAsync(x => x.Id == request.PetId, cancellationToken)) throw new ArgumentException("Animal informado não existe.");
        if (!await work.Usuarios.AnyAsync(x => x.Id == request.VeterinarioId && x.TipoUsuario == "VETERINARIO", cancellationToken))
            throw new ArgumentException("Veterinário informado não existe ou não possui perfil VETERINARIO.");
    }

    public async Task<Agendamento> CreateAsync(AgendamentoRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateReferences(request, cancellationToken);
        var appointment = new Agendamento(request.DataAgendamento, request.Status, request.PetId, request.VeterinarioId);
        work.Agendamentos.Add(appointment);
        await work.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task<Agendamento> UpdateAsync(Guid id, AgendamentoRequest request, CancellationToken cancellationToken = default)
    {
        var appointment = await work.Agendamentos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException("Agendamento não encontrado.");
        await ValidateReferences(request, cancellationToken);
        appointment.Atualizar(request.DataAgendamento, request.Status, request.PetId, request.VeterinarioId);
        await work.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await work.Agendamentos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException("Agendamento não encontrado.");
        work.Agendamentos.Remove(appointment);
        await work.SaveChangesAsync(cancellationToken);
    }
}
