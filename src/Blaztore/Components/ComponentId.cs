namespace Blaztore.Components;

public record ComponentId(string Value)
{
    internal static ComponentId FromNameAndNumber(string name, int count) =>
        new($"{name}-{count}");

    public static ComponentId New() =>
        new(Guid.NewGuid().ToString());
}