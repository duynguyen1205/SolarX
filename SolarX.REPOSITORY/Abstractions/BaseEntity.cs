namespace SolarX.REPOSITORY.Abstractions;

public class BaseEntity<TKey>
{
    public TKey Id { get; set; }

    public bool IsDeleted { get; set; }
}