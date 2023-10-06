using System.Collections.Concurrent;

namespace Blaztore.Components;

public abstract class BaseStateComponent : ExtendedComponentBase, IStateComponent
{
    private static readonly ConcurrentDictionary<string, int> InstanceCounts = new();

    protected BaseStateComponent()
    {
        var name = GetType().Name;
        var count = InstanceCounts.AddOrUpdate(name, 1, (_, value) => value + 1);

        Id = $"{name}-{count}";
    }
    
    public string Id { get; }
    public void ReRender() => InvokeAsync(StateHasChanged);
}