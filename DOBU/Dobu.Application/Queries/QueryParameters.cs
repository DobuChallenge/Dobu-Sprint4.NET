using System.ComponentModel.DataAnnotations;

namespace Dobu.Application.Queries;

public sealed class QueryParameters
{
    [Range(1, 1000000)] public int? Page { get; set; }
    [Range(1, 100)] public int? PageSize { get; set; }
    public string? Sort { get; set; }
    public bool Descending { get; set; }
    [MaxLength(150)] public string? Search { get; set; }
    [MaxLength(40)] public string? Status { get; set; }
    public Guid? PetId { get; set; }
    public Guid? ResponsavelId { get; set; }
    public Guid? VeterinarioId { get; set; }
    public bool Paginated => Page.HasValue || PageSize.HasValue;
    public int CurrentPage => Page ?? 1;
    public int Limit => PageSize ?? 20;

    public void Validate()
    {
        Validator.ValidateObject(this, new ValidationContext(this), true);
    }
}

public interface IQueryMetadata
{
    int Total { get; }
    int Page { get; }
    int PageSize { get; }
    bool Paginated { get; }
}

public sealed class QueryResult<T>(IEnumerable<T> items, int total, QueryParameters parameters) : List<T>(items), IQueryMetadata
{
    public int Total { get; } = total;
    public int Page { get; } = parameters.CurrentPage;
    public int PageSize { get; } = parameters.Limit;
    public bool Paginated { get; } = parameters.Paginated;
}
