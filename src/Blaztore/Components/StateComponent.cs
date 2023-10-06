using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;

namespace Blaztore.Components;

public class StateComponent : ExtendedComponentBase, IStateComponent, IDisposable
{
    private static readonly ConcurrentDictionary<string, int> InstanceCounts = new();

    public StateComponent()
    {
        var name = GetType().Name;
        var count = InstanceCounts.AddOrUpdate(name, 1, (_, value) => value + 1);

        Id = $"{name}-{count}";
    }

    public string Id { get; }
    [Inject] private IActionDispatcher ActionDispatcher { get; set; } = default!;
    [Inject] private IStore Store { get; set; } = default!;
    [Inject] private Subscriptions Subscriptions { get; set; } = default!;

    protected Task Dispatch(IAction action) => ActionDispatcher.Dispatch(action);

    protected T GetState<T>() where T : IState => GetState<T>(DefaultScope.Value);

    protected T GetState<T>(object scope) where T : IState
    {
        var stateType = typeof(T);
        Subscriptions.Add(stateType, scope, this);
        return Store.GetState<T>(scope);
    }

    public void ReRender() => InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        Subscriptions.Remove(this);
        GC.SuppressFinalize(this);
    }
}