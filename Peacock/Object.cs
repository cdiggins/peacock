namespace Peacock;

public interface IObject
{
    Guid Id { get; }
}

public record Object : IObject
{
    public Guid Id { get; init; } = Guid.NewGuid();
}