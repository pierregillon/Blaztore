namespace Blaztore.Components;

public record ComponentId(string Name, int Number)
{
    public override string ToString() => $"{Name}-{Number}";
}