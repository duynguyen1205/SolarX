namespace SolarX.REPOSITORY.Abstractions;

public interface IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; } 
}