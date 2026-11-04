using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Dobu.Domain.Entities;

namespace Dobu.Application.Services;

public record CameraObservationRequest(string Tipo, string Descricao, DateTime Data, double? Confianca);

public sealed class CameraObservationService(IDobuUnitOfWork work, ICameraObservationRepository observations)
{
    private async Task<DobuCam> RequireCamera(Guid cameraId, Guid userId, CancellationToken cancellationToken)
    {
        return await work.DobuCams.FirstOrDefaultAsync(x => x.Id == cameraId && x.Pet.ResponsavelId == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Câmera não encontrada ou sem permissão.");
    }

    public async Task<CameraObservation> CreateAsync(Guid cameraId, Guid userId, CameraObservationRequest request, CancellationToken cancellationToken)
    {
        await RequireCamera(cameraId, userId, cancellationToken);
        var observation = new CameraObservation(cameraId, userId, request.Tipo, request.Descricao, request.Data, request.Confianca);
        if (observation.Timestamp > DateTime.UtcNow.AddMinutes(5)) throw new ArgumentException("A observação não pode estar no futuro.");
        await observations.AddAsync(observation, cancellationToken);
        return observation;
    }

    public async Task<QueryResult<CameraObservation>> ListAsync(Guid cameraId, Guid userId, QueryParameters query, string? type, CancellationToken cancellationToken)
    {
        await RequireCamera(cameraId, userId, cancellationToken);
        query.Validate();
        return await observations.ListAsync(cameraId, query, type, cancellationToken);
    }

    public async Task<CameraObservation> GetAsync(Guid cameraId, Guid userId, Guid id, CancellationToken cancellationToken)
    {
        await RequireCamera(cameraId, userId, cancellationToken);
        return await observations.GetAsync(cameraId, id, cancellationToken) ?? throw new KeyNotFoundException("Observação não encontrada.");
    }

    public async Task DeleteAsync(Guid cameraId, Guid userId, Guid id, CancellationToken cancellationToken)
    {
        await RequireCamera(cameraId, userId, cancellationToken);
        if (!await observations.DeleteAsync(cameraId, id, cancellationToken)) throw new KeyNotFoundException("Observação não encontrada.");
    }
}
