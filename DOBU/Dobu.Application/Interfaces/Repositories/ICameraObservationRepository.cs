using Dobu.Domain.Entities;
using Dobu.Application.Queries;

namespace Dobu.Application.Interfaces.Repositories;

public interface ICameraObservationRepository
{
    Task AddAsync(CameraObservation observation, CancellationToken cancellationToken);
    Task<QueryResult<CameraObservation>> ListAsync(Guid cameraId, QueryParameters parameters, string? type, CancellationToken cancellationToken);
    Task<CameraObservation?> GetAsync(Guid cameraId, Guid id, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid cameraId, Guid id, CancellationToken cancellationToken);
    Task PingAsync(CancellationToken cancellationToken);
}
