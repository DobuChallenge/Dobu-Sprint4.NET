namespace Dobu.Domain.Commons;
 
public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
 
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
 
    public bool Active { get; protected set; } = true;
}