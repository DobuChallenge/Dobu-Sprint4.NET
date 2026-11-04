namespace Dobu.Domain.Entities;

public sealed record CameraObservation
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CameraId { get; init; }
    public Guid OwnerId { get; init; }
    public string Type { get; init; }
    public string Description { get; init; }
    public DateTime Timestamp { get; init; }
    public double? Confidence { get; init; }

    public CameraObservation(Guid cameraId, Guid ownerId, string type, string description, DateTime timestamp, double? confidence)
    {
        if (cameraId == Guid.Empty || ownerId == Guid.Empty) throw new ArgumentException("Câmera e responsável são obrigatórios.");
        Type = (type ?? "").Trim().ToUpperInvariant();
        if (!new[] { "PRESENCA", "MOVIMENTO", "ALIMENTACAO", "HIDRATACAO", "DESCANSO" }.Contains(Type)) throw new ArgumentException("Tipo de observação inválido.");
        Description = (description ?? "").Trim();
        if (Description.Length is < 3 or > 500) throw new ArgumentException("Descreva a observação com 3 a 500 caracteres.");
        if (confidence.HasValue && (!double.IsFinite(confidence.Value) || confidence < 0 || confidence > 1)) throw new ArgumentException("A confiança deve estar entre zero e um.");
        if (timestamp == default) throw new ArgumentException("Informe a data da observação.");
        CameraId = cameraId; OwnerId = ownerId; Confidence = confidence; Timestamp = timestamp.ToUniversalTime();
    }
}
