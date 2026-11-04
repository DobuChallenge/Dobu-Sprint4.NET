using Dobu.Domain.Commons;
using Dobu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Infrastructure.Repositories;

public class Repository<T>(DobuDbContext context) where T : BaseEntity
{
    protected readonly DobuDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public IReadOnlyCollection<T> GetAll()
    {
        return DbSet.AsNoTracking().ToList();
    }

    public T? GetById(Guid id)
    {
        return DbSet.FirstOrDefault(entity => entity.Id == id);
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    public void SaveChanges()
    {
        Context.SaveChanges();
    }
}
