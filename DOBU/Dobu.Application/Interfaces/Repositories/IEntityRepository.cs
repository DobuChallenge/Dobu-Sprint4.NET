using System.Linq.Expressions;
using Dobu.Domain.Commons;
using Dobu.Application.Queries;

namespace Dobu.Application.Interfaces.Repositories;

public interface IEntityRepository<T> where T : BaseEntity
{
    Task<QueryResult<T>> ListAsync(QueryParameters parameters, Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>[]? includes = null, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default,
        Expression<Func<T, object>>[]? includes = null, bool tracking = true);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    void Add(T entity);
    void Remove(T entity);
}
