namespace Domain.Common;

public abstract class DefaultEntity
{
    public Ulid Id { get; set; } = Ulid.NewUlid();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public abstract class DefaultEntity<T>
{
    public T Id { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public abstract class BaseEntity() : AggregateRoot, IAuditable
{
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public abstract class BaseEntity<T> : DefaultEntity<T>, IAuditable
{
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public interface IAuditable
{
    public string CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
