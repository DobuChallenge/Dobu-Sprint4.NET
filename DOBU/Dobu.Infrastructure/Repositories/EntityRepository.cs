using System.Linq.Expressions;
using System.Reflection;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Dobu.Domain.Commons;
using Dobu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Infrastructure.Repositories;

public sealed class EntityRepository<T>(DobuDbContext context) : IEntityRepository<T> where T : BaseEntity
{
    public async Task<QueryResult<T>> ListAsync(QueryParameters parameters, Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>[]? includes = null, CancellationToken cancellationToken = default)
    {
        parameters.Validate();
        IQueryable<T> query = context.Set<T>().AsNoTracking();
        if (predicate is not null) query = query.Where(predicate);
        if (includes is not null) foreach (var include in includes) query = query.Include(include);
        query = ApplyFilters(query, parameters);
        var total = await query.CountAsync(cancellationToken);
        var property = Property(parameters.Sort ?? nameof(BaseEntity.CreatedAt));
        var parameter = Expression.Parameter(typeof(T), "item");
        var selector = Expression.Lambda(Expression.Property(parameter, property), parameter);
        var order = parameters.Descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        query = query.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), order,
            [typeof(T), property.PropertyType], query.Expression, Expression.Quote(selector)));
        if (property.Name != nameof(BaseEntity.Id)) query = ((IOrderedQueryable<T>)query).ThenBy(item => item.Id);
        if (parameters.Paginated) query = query.Skip((parameters.CurrentPage - 1) * parameters.Limit).Take(parameters.Limit);
        return new QueryResult<T>(await query.ToListAsync(cancellationToken), total, parameters);
    }

    private static IQueryable<T> ApplyFilters(IQueryable<T> query, QueryParameters options)
    {
        var item = Expression.Parameter(typeof(T), "item");
        foreach (var (name, value) in new (string, object?)[] { ("Status", options.Status), ("PetId", options.PetId),
                     ("ResponsavelId", options.ResponsavelId), ("VeterinarioId", options.VeterinarioId) })
        {
            if (value is null) continue;
            var property = Property(name);
            var member = Expression.Property(item, property);
            query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Constant(value, property.PropertyType)), item));
        }
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            Expression? filter = null;
            foreach (var property in typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string) && p.Name != "Senha"))
            {
                var member = Expression.Property(item, property);
                var contains = Expression.AndAlso(Expression.NotEqual(member, Expression.Constant(null, typeof(string))),
                    Expression.Call(Expression.Call(member, nameof(string.ToLower), null), nameof(string.Contains), null,
                        Expression.Constant(options.Search.Trim().ToLowerInvariant())));
                filter = filter is null ? contains : Expression.OrElse(filter, contains);
            }
            if (filter is not null) query = query.Where(Expression.Lambda<Func<T, bool>>(filter, item));
        }
        return query;
    }

    private static PropertyInfo Property(string name) => typeof(T).GetProperties()
        .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
            && p.Name != "Senha" && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)))
        ?? throw new ArgumentException($"Campo de consulta inválido: {name}.");

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default,
        Expression<Func<T, object>>[]? includes = null, bool tracking = true)
    {
        IQueryable<T> query = context.Set<T>();
        if (!tracking) query = query.AsNoTracking();
        if (includes is not null) foreach (var include in includes) query = query.Include(include);
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }
    public Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => context.Set<T>().CountAsync(predicate, cancellationToken);
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => context.Set<T>().AnyAsync(predicate, cancellationToken);
    public void Add(T entity) => context.Set<T>().Add(entity);
    public void Remove(T entity) => context.Set<T>().Remove(entity);
}
