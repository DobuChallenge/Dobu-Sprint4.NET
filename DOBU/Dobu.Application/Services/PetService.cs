using Dobu.Application.Interfaces.Repositories;
using Dobu.Domain.Entities;

namespace Dobu.Application.Services;

public record PetRequest(string Nome, int Idade, Guid RacaId, Guid ResponsavelId);

public sealed class PetService(IDobuUnitOfWork work)
{
    private async Task ValidateReferences(PetRequest request, CancellationToken cancellationToken)
    {
        if (!await work.Racas.AnyAsync(x => x.Id == request.RacaId, cancellationToken)) throw new ArgumentException("Raça informada não existe.");
        if (!await work.Usuarios.AnyAsync(x => x.Id == request.ResponsavelId && x.TipoUsuario == "RESPONSAVEL", cancellationToken))
            throw new ArgumentException("Responsável informado não existe ou não possui perfil RESPONSAVEL.");
    }

    public async Task<Pet> CreateAsync(PetRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateReferences(request, cancellationToken);
        var pet = new Pet(request.Nome, request.Idade, request.RacaId, request.ResponsavelId);
        work.Pets.Add(pet);
        await work.SaveChangesAsync(cancellationToken);
        return pet;
    }

    public async Task<Pet> UpdateAsync(Guid id, PetRequest request, CancellationToken cancellationToken = default)
    {
        var pet = await work.Pets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException("Animal não encontrado.");
        await ValidateReferences(request, cancellationToken);
        pet.Atualizar(request.Nome, request.Idade, request.RacaId, request.ResponsavelId);
        await work.SaveChangesAsync(cancellationToken);
        return pet;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pet = await work.Pets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException("Animal não encontrado.");
        work.Pets.Remove(pet);
        await work.SaveChangesAsync(cancellationToken);
    }
}
