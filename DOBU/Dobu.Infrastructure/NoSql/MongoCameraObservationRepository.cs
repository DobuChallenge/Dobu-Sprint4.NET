using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Dobu.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Dobu.Infrastructure.NoSql;

public sealed class MongoCameraObservationRepository(IMongoDatabase database) : ICameraObservationRepository
{
    private readonly IMongoCollection<ObservationDocument> _collection = database.GetCollection<ObservationDocument>("camera_observations");

    public async Task AddAsync(CameraObservation observation, CancellationToken cancellationToken)
    {
        await EnsureIndexesAsync(cancellationToken);
        await _collection.InsertOneAsync(ObservationDocument.From(observation), cancellationToken: cancellationToken);
    }

    public async Task<QueryResult<CameraObservation>> ListAsync(Guid cameraId, QueryParameters parameters, string? type, CancellationToken cancellationToken)
    {
        parameters.Validate();
        var filter = Builders<ObservationDocument>.Filter.Eq(x => x.CameraId, cameraId.ToString());
        if (!string.IsNullOrWhiteSpace(type)) filter &= Builders<ObservationDocument>.Filter.Eq(x => x.Type, type.Trim().ToUpperInvariant());
        if (!string.IsNullOrWhiteSpace(parameters.Search)) filter &= Builders<ObservationDocument>.Filter.Regex(x => x.Description, new BsonRegularExpression(System.Text.RegularExpressions.Regex.Escape(parameters.Search.Trim()), "i"));
        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var sorted = _collection.Find(filter).Sort(parameters.Descending
            ? Builders<ObservationDocument>.Sort.Descending(x => x.Timestamp).Descending(x => x.Id)
            : Builders<ObservationDocument>.Sort.Ascending(x => x.Timestamp).Ascending(x => x.Id));
        if (parameters.Paginated) sorted = sorted.Skip((parameters.CurrentPage - 1) * parameters.Limit).Limit(parameters.Limit);
        var records = await sorted.ToListAsync(cancellationToken);
        return new QueryResult<CameraObservation>(records.Select(x => x.ToDomain()), checked((int)total), parameters);
    }

    public async Task<CameraObservation?> GetAsync(Guid cameraId, Guid id, CancellationToken cancellationToken) =>
        (await _collection.Find(x => x.CameraId == cameraId.ToString() && x.Id == id.ToString()).FirstOrDefaultAsync(cancellationToken))?.ToDomain();

    public async Task<bool> DeleteAsync(Guid cameraId, Guid id, CancellationToken cancellationToken) =>
        (await _collection.DeleteOneAsync(x => x.CameraId == cameraId.ToString() && x.Id == id.ToString(), cancellationToken)).DeletedCount == 1;

    public async Task PingAsync(CancellationToken cancellationToken) =>
        await database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);

    public async Task EnsureIndexesAsync(CancellationToken cancellationToken) =>
        await _collection.Indexes.CreateOneAsync(new CreateIndexModel<ObservationDocument>(Builders<ObservationDocument>.IndexKeys.Ascending(x => x.CameraId).Descending(x => x.Timestamp)), cancellationToken: cancellationToken);

    private sealed class ObservationDocument
    {
        [BsonId] public string Id { get; set; } = "";
        public string CameraId { get; set; } = "";
        public string OwnerId { get; set; } = "";
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public double? Confidence { get; set; }
        public static ObservationDocument From(CameraObservation item) => new() { Id = item.Id.ToString(), CameraId = item.CameraId.ToString(), OwnerId = item.OwnerId.ToString(), Type = item.Type, Description = item.Description, Timestamp = item.Timestamp, Confidence = item.Confidence };
        public CameraObservation ToDomain() => new(Guid.Parse(CameraId), Guid.Parse(OwnerId), Type, Description, Timestamp, Confidence) { Id = Guid.Parse(Id) };
    }
}
